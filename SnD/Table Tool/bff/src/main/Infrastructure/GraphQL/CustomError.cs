namespace Infrastructure.GraphQL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using HotChocolate;

    // Excluded because it is just a wrapper on top of an existing IError instance
    [ExcludeFromCodeCoverage]
    public class CustomError : IError
    {
        private IError error;

        public CustomError(string code, string message)
        {
            error = ErrorBuilder.New().SetCode(code).SetMessage(message).Build();
        }

        public string Message => error.Message;

        public string? Code => error.Code;

        public IReadOnlyList<object>? Path => error.Path?.ToList();

        public IReadOnlyList<Location>? Locations => error.Locations;

        public Exception? Exception => error.Exception;

        public IReadOnlyDictionary<string, object?>? Extensions => error.Extensions;

        Path? IError.Path => error.Path;

        public IError WithMessage(string message)
        {
            error = error.WithMessage(message);
            return this;
        }

        public IError WithCode(string? code)
        {
            error = error.WithCode(code);
            return this;
        }

        public IError WithPath(Path? path)
        {
            error = error.WithPath(path);
            return this;
        }

        public IError WithPath(IReadOnlyList<object>? path)
        {
            error = error.WithPath(path);
            return this;
        }

        public IError WithLocations(IReadOnlyList<Location>? locations)
        {
            error = error.WithLocations(locations);
            return this;
        }

        public IError WithException(Exception? exception)
        {
            error = error.WithException(exception);
            return this;
        }

        public IError RemoveException()
        {
            error = error.RemoveException();
            return this;
        }

        public IError WithExtensions(IReadOnlyDictionary<string, object?> extensions)
        {
            error = error.WithExtensions(extensions);
            return this;
        }

        public IError SetExtension(string key, object? value)
        {
            error = error.SetExtension(key, value);
            return this;
        }

        public IError RemoveExtension(string key)
        {
            error = error.RemoveExtension(key);
            return this;
        }

        public IError RemoveCode()
        {
            // Not implemented
            return this;
        }

        public IError RemovePath()
        {
            // Not implemented
            return this;
        }

        public IError RemoveLocations()
        {
            // Not implemented
            return this;
        }

        public IError RemoveExtensions()
        {
            // Not implemented
            return this;
        }
    }
}
