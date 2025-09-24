namespace MongoDbSeeder
{
    using System.IO;
    using System.Reflection;

    public static class JsonFileLoader
    {
        public static string LoadFile(string fileName)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "json_files", fileName);
            return File.ReadAllText(path);
        }
    }
}
