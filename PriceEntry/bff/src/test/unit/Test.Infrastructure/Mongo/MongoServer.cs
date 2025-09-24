namespace Test.Infrastructure.Mongo
{
    using System;
    using System.Runtime.CompilerServices;

    using EphemeralMongo;

    public static class MongoServer
    {
        public static IMongoRunner Runner { get; private set; } = null!;

        [ModuleInitializer]
        public static void Initialize()
        {
            Runner = MongoRunnerProvider.Get();

            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Runner.Dispose();
        }
    }
}
