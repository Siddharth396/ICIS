namespace Architecture.Tests
{
    using System.Reflection;

    using Authoring.Infrastructure;

    public static class SolutionAssemblies
    {
        public static readonly Assembly AuthoringBff = typeof(Program).Assembly;

        public static readonly Assembly SubscriberBff = typeof(Subscriber.Infrastructure.Program).Assembly;

        public static readonly Assembly BusinessLayer = typeof(BusinessLayer.Registration.ServicesRegistration).Assembly;
    }
}
