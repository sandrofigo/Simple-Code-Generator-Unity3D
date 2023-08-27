using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Serilog;

[ParameterPrefix(nameof(ChangelogVersionMatchesGitTagVersion))]
interface IChangelogVersionMatchesGitTagVersion : INukeBuild
{
    [GitRepository] private GitRepository GitRepository => TryGetValue(() => GitRepository);

    [Parameter] AbsolutePath ChangelogFile => TryGetValue(() => ChangelogFile);

    Target ChangelogVersionMatchesGitTagVersion => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .Executes(() =>
        {
            SemanticVersion versionFromChangelog = GetLatestVersionFromChangelog(ChangelogFile);
            SemanticVersion versionFromGitTag = GitRepository.GetLatestVersionTagOnCurrentCommit();

            Assert.True(versionFromChangelog == versionFromGitTag, $"The version {versionFromChangelog} from the changelog does not match the version {versionFromGitTag} from the git tag!");

            Log.Information("Changelog version and git tag version match");
        });

    static SemanticVersion GetLatestVersionFromChangelog(AbsolutePath pathToChangelogFile)
    {
        Assert.True(pathToChangelogFile != null, "No path has been provided!");

        return ChangelogTasks.ReadChangelog(pathToChangelogFile).GetLatestReleaseNotes()?.Version;
    }
}