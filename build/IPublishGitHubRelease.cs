using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Serilog;

[ParameterPrefix(nameof(PublishGitHubRelease))]
interface IPublishGitHubRelease : INukeBuild
{
    [GitRepository] private GitRepository GitRepository => TryGetValue(() => GitRepository);

    Target PublishGitHubRelease => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .OnlyWhenStatic(() => IsServerBuild)
        .DependsOn<IUnityPackageVersionMatchesGitTagVersion>()
        .DependsOn<IChangelogVersionMatchesGitTagVersion>()
        .Executes(() =>
        {
            Log.Warning("TODO");
        });
}