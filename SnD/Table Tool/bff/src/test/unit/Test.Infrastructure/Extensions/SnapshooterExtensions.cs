namespace Tests.Infrastructure.Extensions
{
    using System;
    using Snapshooter;
    using Snapshooter.Xunit;

    public static class SnapshooterExtensions
    {
        public static void MatchSnapshot(this object currentResult, string snapshotName, string ignoreFieldsPath)
        {
            Func<FieldOption, object> fieldOption = fieldOption => fieldOption.Fields<string>(ignoreFieldsPath);
            currentResult.MatchSnapshot(snapshotName, matchOptions => matchOptions.Ignore(fieldOption));
        }
    }
}