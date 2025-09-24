namespace Infrastructure.Services.Workflow
{
    using System;

    public record OperationType(string Value)
    {
        public static readonly OperationType Correction = new("correction");

        public static readonly OperationType None = new(string.Empty);

        public bool Matches(string? value)
        {
            return string.Equals(Value, value, StringComparison.OrdinalIgnoreCase);
        }

        public static OperationType Parse(string? value)
        {
            return Correction.Matches(value) ? Correction : None;
        }
    }
}
