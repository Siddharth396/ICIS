namespace Infrastructure.Attributes.BusinessAnnotations
{
    using System;

    /// <summary>
    /// Indicates that the decorated class is an Application Service.
    /// An Application Service is responsible for orchestrating the application's use cases and coordinating tasks.
    /// Responsibilities include:
    /// - Handling application logic and use cases.
    /// - Coordinating tasks between domain services, and other components.
    /// - Serving as a boundary between the domain model and the external world (e.g., UI, API).
    /// - Delegating domain-specific logic to domain services and entities.
    /// - Ensuring that business rules and policies are enforced.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ApplicationServiceAttribute : Attribute;
}
