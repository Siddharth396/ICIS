namespace Infrastructure.Services.Workflow
{
    using System.Collections.Generic;

    public static class UserActions
    {
        public const string Approve = "APPROVE";

        public const string InitiateCorrection = "INITIATE_CORRECTION";

        public const string Publish = "PUBLISH";

        public const string PullBack = "PULL_BACK";

        public const string ReleaseReview = "RELEASE_REVIEW";

        public const string SendBack = "SEND_BACK";

        public const string SendForReview = "SEND_FOR_REVIEW";

        public const string StartReview = "START_REVIEW";

        public const string Cancel = "CANCEL";

        public const string StartNma = "START_NMA";

        public const string CancelNma = "CANCEL_NMA";

        private static readonly IReadOnlySet<string> EditorActions = new HashSet<string>
        {
            InitiateCorrection,
            Publish,
            PullBack,
            SendForReview,
            Cancel,
            StartNma,
            CancelNma
        };

        private static readonly IReadOnlySet<string> CopyEditorActions = new HashSet<string>
        {
            Approve,
            SendBack,
            StartReview,
            ReleaseReview,
            Cancel
        };

        public static bool IsEditorAction(string action)
        {
            return EditorActions.Contains(action);
        }

        public static bool IsCopyEditorAction(string action)
        {
            return CopyEditorActions.Contains(action);
        }
    }
}
