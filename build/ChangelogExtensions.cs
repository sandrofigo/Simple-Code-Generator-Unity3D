using System.Linq;
using Nuke.Common.ChangeLog;
using Nuke.Common.Utilities.Collections;

public static class ChangelogExtensions
{
    public static ReleaseNotes GetLatestReleaseNotes(this ChangeLog changelog)
    {
        return changelog.ReleaseNotes.IsEmpty() ? null : changelog.ReleaseNotes.MaxBy(n => n.Version);
    }
}