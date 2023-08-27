using System;
using System.Linq;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Serilog;

[ParameterPrefix(nameof(PublishGitHubRelease))]
interface IPublishGitHubRelease : INukeBuild
{
    [GitRepository] private GitRepository GitRepository => TryGetValue(() => GitRepository);

    [Parameter] AbsolutePath ChangelogFile => TryGetValue(() => ChangelogFile);

    Target PublishGitHubRelease => _ => _
        // .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        // .OnlyWhenStatic(() => IsServerBuild)
        // .DependsOn<IUnityPackageVersionMatchesGitTagVersion>()
        // .DependsOn<IChangelogVersionMatchesGitTagVersion>()
        .Executes(async () =>
        {
            Assert.True(ChangelogFile != null, "No path has been provided!");

            ChangeLog changelog = ChangelogTasks.ReadChangelog(ChangelogFile);
            ReleaseNotes latestReleaseNotes = changelog.GetLatestReleaseNotes();
            var trimmedNotes = latestReleaseNotes.Notes.SkipUntil(n => !string.IsNullOrWhiteSpace(n)).Reverse().SkipUntil(n => !string.IsNullOrWhiteSpace(n)).Reverse();

            string changelogBody = string.Join(Environment.NewLine, trimmedNotes);

            SemanticVersion version = GitRepository.GetLatestVersionTagOnCurrentCommit();

            var release = new NewRelease($"v{version}")
            {
                Draft = true,
                Name = $"v{version}",
                Prerelease = version.IsPrerelease,
                Body = changelogBody
            };

            string owner = GitRepository.GetGitHubOwner();
            string name = GitRepository.GetGitHubName();

            var credentials = new Credentials(GitHubActions.Instance.Token);
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new Octokit.Internal.InMemoryCredentialStore(credentials));

            Log.Information("Creating GitHub release...");

            Release createdRelease = await GitHubTasks.GitHubClient.Repository.Release.Create(owner, name, release);

            // await GitHubTasks.GitHubClient.Repository.Release.Edit(owner, name, createdRelease.Id, new ReleaseUpdate { Draft = false });
        });
}