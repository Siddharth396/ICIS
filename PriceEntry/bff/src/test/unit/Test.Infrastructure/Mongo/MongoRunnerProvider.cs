namespace Test.Infrastructure.Mongo
{
    using System;

    using EphemeralMongo;

    /// <summary>
    /// https://gist.github.com/asimmon/612b2d54f1a0d2b4e1115590d456e0be
    /// which has a link to this https://gist.github.com/asimmon/612b2d54f1a0d2b4e1115590d456e0be
    /// </summary>
    public static class MongoRunnerProvider
    {
        private static readonly object LockObj = new();

        private static IMongoRunner? runner;

        private static int useCounter;

        public static IMongoRunner Get()
        {
            lock (LockObj)
            {
                runner ??= MongoRunner.Run(
                    new MongoRunnerOptions
                    {
                        UseSingleNodeReplicaSet = true,
                        AdditionalArguments = "--quiet",
                        KillMongoProcessesWhenCurrentProcessExits = true
                    });
                useCounter++;
                return new MongoRunnerWrapper(runner);
            }
        }

        private sealed class MongoRunnerWrapper : IMongoRunner
        {
            private IMongoRunner? underlyingMongoRunner;

            public MongoRunnerWrapper(IMongoRunner underlyingMongoRunner)
            {
                this.underlyingMongoRunner = underlyingMongoRunner;
            }

            public string ConnectionString =>
                underlyingMongoRunner?.ConnectionString ?? throw new ObjectDisposedException(nameof(IMongoRunner));

            public void Dispose()
            {
                if (underlyingMongoRunner != null)
                {
                    underlyingMongoRunner = null;
                    StaticDispose();
                }
            }

            public void Export(
                string database,
                string collection,
                string outputFilePath,
                string? additionalArguments = null)
            {
                if (underlyingMongoRunner == null)
                {
                    throw new ObjectDisposedException(nameof(IMongoRunner));
                }

                underlyingMongoRunner.Export(
                    database,
                    collection,
                    outputFilePath,
                    additionalArguments);
            }

            public void Import(
                string database,
                string collection,
                string inputFilePath,
                string? additionalArguments = null,
                bool drop = false)
            {
                if (underlyingMongoRunner == null)
                {
                    throw new ObjectDisposedException(nameof(IMongoRunner));
                }

                underlyingMongoRunner.Import(
                    database,
                    collection,
                    inputFilePath,
                    additionalArguments,
                    drop);
            }

            private static void StaticDispose()
            {
                lock (LockObj)
                {
                    if (runner != null)
                    {
                        useCounter--;
                        if (useCounter == 0)
                        {
                            runner.Dispose();
                            runner = null;
                        }
                    }
                }
            }
        }
    }
}