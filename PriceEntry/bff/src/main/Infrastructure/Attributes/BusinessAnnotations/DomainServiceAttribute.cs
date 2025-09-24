namespace Infrastructure.Attributes.BusinessAnnotations
{
    using System;

    /// <summary>
    /// Indicates that the decorated class is a Domain Service.
    /// A Domain Service is responsible for encapsulating domain logic that does not naturally fit within an Entity or Value Object.
    /// Responsibilities include:
    /// - Encapsulating domain logic involving multiple entities or value objects.
    /// - Being stateless and operating on the state of entities and value objects.
    /// - Coordinating complex operations that span across different parts of the domain model.
    /// - Implementing business rules that are not specific to a single entity.
    /// - Interfacing with repositories to retrieve and persist entities.
    /// - Providing domain-specific functionality used by application services or other parts of the system.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DomainServiceAttribute : Attribute;
}
