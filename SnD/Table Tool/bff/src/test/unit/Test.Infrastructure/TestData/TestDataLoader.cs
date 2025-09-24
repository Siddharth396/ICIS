namespace Test.Infrastructure.TestData
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    public static class TestDataLoader
    {
        public static IEnumerable<string> LoadJson(string json)
        {
            var settings = new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore
            };
            var objects = JArray.Parse(json, settings);
            return objects.Select(x => x.ToString());
        }


    }
}