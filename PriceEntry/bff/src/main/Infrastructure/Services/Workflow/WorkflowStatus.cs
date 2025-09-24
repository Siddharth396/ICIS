namespace Infrastructure.Services.Workflow
{
    using System;
    using System.Collections.Generic;

    using Icis.Workflow;
    using Icis.Workflow.Extensions;

    public record WorkflowStatus(string Value)
    {
        public static readonly WorkflowStatus None = new(string.Empty);

        public static readonly WorkflowStatus Published = new(Status.PUBLISHED.ToString());

        public static readonly WorkflowStatus Draft = new(Status.DRAFT.ToString());

        public static readonly WorkflowStatus ReturnedToAuthor = new(Status.RETURNED_TO_AUTHOR.ToString());

        public static readonly WorkflowStatus ReadyForReview = new(Status.READY_FOR_REVIEW.ToString());

        public static readonly WorkflowStatus InReview = new(Status.IN_REVIEW.ToString());

        public static readonly WorkflowStatus CorrectionDraft = new(Status.CORRECTION_DRAFT.ToString());

        public static readonly WorkflowStatus CorrectionReadyForReview = new(Status.CORRECTION_READY_FOR_REVIEW.ToString());

        public static readonly WorkflowStatus CorrectionInReview = new(Status.CORRECTION_IN_REVIEW.ToString());

        public static readonly WorkflowStatus CorrectionReadyToPublish = new(Status.CORRECTION_READY_TO_PUBLISH.ToString());

        public static readonly WorkflowStatus CorrectionPublished = new(Status.CORRECTION_PUBLISHED.ToString());

        public static readonly WorkflowStatus CorrectionPublishInProgress = new(Status.CORRECTION_PUBLISH_IN_PROGRESS.ToString());

        public static readonly WorkflowStatus ReadyToPublish = new(Status.READY_TO_PUBLISH.ToString());

        public static readonly WorkflowStatus Cancelled = new(Status.CANCELLED.ToString());

        public bool Matches(string? value)
        {
            return string.Equals(Value, value, StringComparison.OrdinalIgnoreCase);
        }

        public static string GetDisplayValueForStatus(string? status)
        {
            if (!string.IsNullOrEmpty(status) && Enum.TryParse(status, out Status result))
            {
                return result.GetDisplayName().ToUpperInvariant();
            }

            return "READY TO START";
        }

        public static bool IsCorrectionPrePublishStatus(WorkflowStatus status)
        {
            return status == CorrectionDraft
                   || status == CorrectionReadyForReview
                   || status == CorrectionInReview
                   || status == CorrectionReadyToPublish
                   || status == CorrectionPublishInProgress;
        }

        public static bool IsCorrectionStatus(WorkflowStatus status)
        {
            return status == CorrectionDraft
                   || status == CorrectionReadyForReview
                   || status == CorrectionInReview
                   || status == CorrectionReadyToPublish
                   || status == CorrectionPublished
                   || status == CorrectionPublishInProgress
                   || status == Cancelled;
        }

        public bool IsEditorAllowedToEdit()
        {
            return this == Draft || this == CorrectionDraft || this == ReturnedToAuthor;
        }

        public bool IsCopyEditorAllowedToEdit()
        {
            return this == InReview || this == CorrectionInReview;
        }

        public static bool IsPublishedOrCorrectionPublishedStatusMatch(string? status)
        {
            var validStatuses = new List<string>() { Published.Value, CorrectionPublished.Value };
            return !string.IsNullOrEmpty(status) ? validStatuses.Contains(status) : false;
        }
    }
}