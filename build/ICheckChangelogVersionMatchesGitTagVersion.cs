using Nuke.Common;
using Nuke.Common.Git;
using Serilog;

[ParameterPrefix(nameof(CheckChangelogVersionMatchesGitTagVersion))]
interface ICheckChangelogVersionMatchesGitTagVersion : INukeBuild
{
    [GitRepository] private GitRepository GitRepository => TryGetValue(() => GitRepository);
    
    Target CheckChangelogVersionMatchesGitTagVersion => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .Executes(() =>
        {
            Log.Warning("TODO");
        });
}