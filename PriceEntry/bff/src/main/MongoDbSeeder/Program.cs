namespace MongoDbSeeder
{
    using System;
    using System.IO;
    using System.Text;

    using dotenv.net;

    using Infrastructure.Configuration;
    using Infrastructure.Configuration.Model;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using MongoDB.Driver;

    internal class Program
    {
        public Program()
            : this("appsettings.json", ".env")
        {
        }

        public Program(
            string appSettingsPath,
            string envFilename)
        {
            AppSettingsPath = appSettingsPath;
            EnvFilename = envFilename;
        }

        public string AppSettingsPath { get; set; }

        public string EnvFilename { get; set; }

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting seeding...");
            using var host = Host.CreateDefaultBuilder(args).Build();

            var program = new Program();
            var configuration = program.GetConfiguration();

            var database = GetMongo(configuration);

            ExecuteWithoutBreakingOnException(
               () =>
               {
                   if (DatabaseSeeder.ShouldSeederRun(database))
                   {
                       Console.WriteLine("Inserting items to mongodb...");
                       DatabaseSeeder.SeedAll(database);
                       Console.WriteLine("All items have been inserted");
                   }
               });

            static void ExecuteWithoutBreakingOnException(Action action)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            static IMongoDatabase GetMongo(IConfiguration config)
            {
                var options = config.GetOptions<MongoDbOptions>();

                var mongoDbConnectionString = Environment.GetEnvironmentVariable("MONGO_DB_CONNECTION_STRING");
                var mongoDbUser = Environment.GetEnvironmentVariable("MONGO_DB_USER");
                var mongoDbPwd = Environment.GetEnvironmentVariable("MONGO_DB_PWD");

                if (mongoDbConnectionString != null && mongoDbUser != null && mongoDbPwd != null)
                {
                    options.ConnectionString = mongoDbConnectionString;
                    options.Username = mongoDbUser;
                    config["MongoDb:Password"] = mongoDbPwd;
                }

                var mongoUrl = MongoUrl.Create(options.BuildConnectionString(config));

                var settings = MongoClientSettings.FromUrl(mongoUrl);

                var client = new MongoClient(settings);

                return client.GetDatabase(
                    mongoUrl.DatabaseName,
                    new MongoDatabaseSettings { ReadPreference = ReadPreference.PrimaryPreferred });
            }
        }

        public IConfiguration GetConfiguration()
        {
            DotEnv.Load(new DotEnvOptions(true, new[] { EnvFilename }, Encoding.UTF8));

            var builder = new ConfigurationBuilder();
            BuildConfiguration(builder);

            return builder.BuildAndReplacePlaceholders();
        }

        public void BuildConfiguration(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile(
                    AppSettingsPath,
                    false,
                    false);

            builder.AddEnvironmentVariables();
        }
    }
}