namespace Infrastructure.Services.Workflow
{
    public record WorkflowId(string Value)
    {
        public static WorkflowId None => new(string.Empty);
    }
}
