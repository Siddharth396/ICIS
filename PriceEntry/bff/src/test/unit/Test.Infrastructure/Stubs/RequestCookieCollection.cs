namespace Test.Infrastructure.Stubs
{
    using System.Collections;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;

    public class RequestCookieCollection : IRequestCookieCollection
    {
        private readonly Dictionary<string, string> internalCookies;

        public RequestCookieCollection()
            : this(new Dictionary<string, string>())
        {
        }

        public RequestCookieCollection(Dictionary<string, string> cookies)
        {
            internalCookies = cookies;
        }

        public int Count => internalCookies.Count;

        public ICollection<string> Keys => internalCookies.Keys;

        public string this[string key] => internalCookies[key];

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return internalCookies.GetEnumerator();
        }

        public bool ContainsKey(string key)
        {
            return internalCookies.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            var result = internalCookies.TryGetValue(key, out var output);
            value = output ?? string.Empty;
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}