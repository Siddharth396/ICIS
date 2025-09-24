namespace BusinessLayer.ContentBlock.DTOs.Output
{
    using System;

    public record NextPublicationDate(DateTime? ScheduledPublishDate, string ScheduleId);
}
