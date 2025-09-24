namespace BusinessLayer.DataPackage.Handler
{
    using BusinessLayer.DataPackage.Repositories.Models;

    using Infrastructure.EventDispatcher;
    using Infrastructure.Services.Workflow;

    public class DataPackageUpdatedEvent : IEvent
    {
        public required DataPackage DataPackage { get; set; }

        public required WorkflowStatus Status { get; set; }

        public required string UserId { get; set; }
    }
}
