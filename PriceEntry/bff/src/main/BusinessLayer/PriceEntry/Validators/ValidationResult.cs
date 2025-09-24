namespace BusinessLayer.PriceEntry.Validators
{
    using System.Collections.Generic;

    public class ValidationResult
    {
        private Dictionary<string, List<string>>? validationErrors;

        public bool IsValid => validationErrors == null || validationErrors.Count == 0;

        public IReadOnlyDictionary<string, List<string>> ValidationErrors =>
            validationErrors ?? new Dictionary<string, List<string>>();

        public void AddError(string key, string value)
        {
            if (validationErrors == null)
            {
                validationErrors = new Dictionary<string, List<string>>();
            }

            // convert key to camel case to match javascript rules
            key = char.ToLowerInvariant(key[0]) + key[1..];

            if (!validationErrors.ContainsKey(key))
            {
                validationErrors[key] = new List<string>();
            }

            validationErrors[key].Add(value);
        }
    }
}
