namespace BusinessLayer.Services.Common
{
    using Microsoft.Extensions.Configuration;

    public class SqlQueryReader(IConfiguration configuration) : ISqlQueryReader
    {
        public string ReadQuery(string scriptName)
        {
            var scriptsPath = configuration.GetSection("scriptsPath").Value;
            var filePath = Path.Combine(scriptsPath, scriptName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Script file '{scriptName}' not found at path '{filePath}'.");
            }

            return File.ReadAllText(filePath);
        }
    }
}
