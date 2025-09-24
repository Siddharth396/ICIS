namespace Infrastructure.MongoDB.Transactions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;

    using Infrastructure.Configuration;
    using Infrastructure.Configuration.Model;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    using Polly;

    using Serilog;

    [ExcludeFromCodeCoverage]
    internal class MongoDbTransactionMiddleware
    {
        private readonly RequestDelegate next;

        public MongoDbTransactionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, MongoContext mongoDbContext, IConfiguration configuration)
        {
            var mongoOptions = configuration.GetOptions<MongoDbOptions>();

            // Define a Polly retry policy to handle transient MongoDB errors.
            // The policy will catch exceptions labeled as "TransientTransactionError" or "UnknownTransactionCommitResult".
            var retryPolicy = Policy
               .Handle<MongoException>(
                    ex => ex.HasErrorLabel("TransientTransactionError")
                          || ex.HasErrorLabel("UnknownTransactionCommitResult"))
               .WaitAndRetryAsync(
                    retryCount: mongoOptions.MaxRetryAttempts,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromMilliseconds(mongoOptions.RetryBaseDelayMilliseconds * Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (
                        exception,
                        timeSpan,
                        retryCount,
                        pollyContext) =>
                    {
                        Log.Warning(
                            exception,
                            $"Retrying transaction: Attempt {retryCount} after {timeSpan.TotalMilliseconds} ms due to {exception.GetType().Name}");
                    });

            MemoryStream? requestBufferingStream = null;
            Stream? originalBody = null;

            try
            {
                var request = httpContext.Request;

                // Store the original request body stream
                originalBody = request.Body;
                requestBufferingStream = new MemoryStream();

                // Copy the request body to a memory stream for buffering
                await request.Body.CopyToAsync(requestBufferingStream);
                requestBufferingStream.Seek(0, SeekOrigin.Begin);

                // Replace the request body with the buffered stream
                httpContext.Request.Body = requestBufferingStream;

                MemoryStream? responseBufferingStream = null;
                Stream? originalResponseBody = null;

                try
                {
                    var response = httpContext.Response;

                    // Store the original response body stream
                    originalResponseBody = response.Body;
                    responseBufferingStream = new MemoryStream();

                    // Replace the response body with a memory stream for buffering
                    response.Body = responseBufferingStream;

                    // Start a MongoDB session which will be used for the transaction.
                    await mongoDbContext.StartSessionAsync();

                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        // Start a new transaction for the current retry attempt.
                        // mongoDbContext.StartTransaction();
                        try
                        {
                            // Reset the request body at the start of each retry
                            requestBufferingStream.Seek(0, SeekOrigin.Begin);
                            httpContext.Request.Body = requestBufferingStream;

                            // Clear the memory stream buffer to avoid accumulating responses from previous attempts
                            responseBufferingStream.SetLength(0);
                            responseBufferingStream.Seek(0, SeekOrigin.Begin);

                            // Execute the next middleware in the pipeline
                            await next(httpContext);

                            // Attempt to commit the transaction.
                            await mongoDbContext.CommitChangesAsync();
                        }
                        catch (MongoException)
                        {
                            // If an exception occurs, abort the transaction to roll back any partial changes.
                            await mongoDbContext.AbortChangesAsync();
                            throw; // This rethrows the exception so Polly can decide whether to retry.
                        }
                    });

                    // Check if the responseBufferingStream is still open and can seek
                    if (responseBufferingStream.CanSeek)
                    {
                        // Copy the buffered response data to the original response stream
                        responseBufferingStream.Seek(0, SeekOrigin.Begin);
                        await responseBufferingStream.CopyToAsync(originalResponseBody);
                    }
                    else
                    {
                        // Handle the case where the stream is closed or non-seekable.
                        using var newResponseBufferingStream = new MemoryStream(responseBufferingStream.ToArray());

                        // Copy the new stream data to the original response stream
                        newResponseBufferingStream.Seek(0, SeekOrigin.Begin);
                        await newResponseBufferingStream.CopyToAsync(originalResponseBody);
                    }
                }
                finally
                {
                    // Dispose the response buffering stream and restore the original response body stream
                    if (responseBufferingStream != null)
                    {
                        await responseBufferingStream.DisposeAsync();
                    }

                    if (originalResponseBody != null)
                    {
                        httpContext.Response.Body = originalResponseBody;
                    }
                }
            }
            finally
            {
                // Dispose the request buffering stream and restore the original request body stream
                if (requestBufferingStream != null)
                {
                    await requestBufferingStream.DisposeAsync();
                }

                if (originalBody != null)
                {
                    httpContext.Request.Body = originalBody;
                }
            }
        }
    }
}
