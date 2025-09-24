namespace BusinessLayer.ContentBlock.ValueObjects
{
    using System;

    public record Version
    {
        public static readonly Version Latest = new(int.MaxValue);

        public Version(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Version cannot be negative");
            }

            Value = value;
        }

        public int Value { get; }

        public static Version From(int value)
        {
            return new Version(value);
        }

        public Version Increment()
        {
            return new Version(Value + 1);
        }
    }
}
