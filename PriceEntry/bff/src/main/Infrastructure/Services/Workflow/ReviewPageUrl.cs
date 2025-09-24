namespace Infrastructure.Services.Workflow
{
    using System;

    public record ReviewPageUrl
    {
        public static readonly ReviewPageUrl Empty = new(string.Empty);

        private ReviewPageUrl(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        public static ReviewPageUrl Create(string path)
        {
            ValidatePath(path);

            return new ReviewPageUrl(path);
        }

        private static void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            // Check if the path is a relative path
            if (!Uri.TryCreate(path, UriKind.Relative, out _))
            {
                throw new ArgumentException("Path must be a relative path.", nameof(path));
            }
        }
    }
}
