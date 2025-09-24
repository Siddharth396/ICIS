namespace Infrastructure.Services.Workflow
{
    public record WorkflowVersion(string Value)
    {
        public static readonly WorkflowVersion Simple = new("simple");

        public static readonly WorkflowVersion Advance = new("advance");
    }
}
