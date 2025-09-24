namespace BusinessLayer.Services.Models
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class Response<T>
    {
        public Error? Error { get; } = null;

        public T? Data { get; } = default;

        public bool IsSuccess => !IsFailure;

        public bool IsFailure => Error is not null;

        private Response(T data)
        {
            Data = data;
        }

        private Response(string errorMessage, string errorCode, T? data = default)
        {
            Error = new Error(errorMessage, errorCode);
            Data = data;
        }

        public static Response<T> Success(T data)
        {
            return new Response<T>(data);
        }

        public static Response<T> Failure(string error, string errorCode)
        {
            return new Response<T>(
                error,
                errorCode);
        }

        public static Response<T> Failure(Error error)
        {
            return new Response<T>(
                error.Message,
                error.Code);
        }
    }
}
