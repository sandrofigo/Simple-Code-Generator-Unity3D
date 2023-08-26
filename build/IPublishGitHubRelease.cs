using Nuke.Common;
using Nuke.Common.Git;
using Serilog;

[ParameterPrefix(nameof(PublishGitHubRelease))]
interface IPublishGitHubRelease : INukeBuild
{
    [GitRepository] private GitRepository GitRepository => TryGetValue(() => GitRepository);

    Target PublishGitHubRelease => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .DependsOn<ICheckChangelogVersionMatchesUnityPackageVersion>()
        .DependsOn<ICheckChangelogVersionMatchesGitTagVersion>()
        .Executes(() =>
        {
            Log.Warning("TODO");
        });
}