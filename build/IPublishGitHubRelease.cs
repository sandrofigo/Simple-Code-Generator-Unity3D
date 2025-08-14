using System.Linq;
using System.Text;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitHub;
using Octokit;
using Octokit.Internal;
using Serilog;

[ParameterPrefix(nameof(PublishGitHubRelease))]
interface IPublishGitHubRelease : INukeBuild
{
    [GitRepository] private GitRepository GitRepository => TryGetValue(() => GitRepository);

    [Nuke.Common.Parameter] AbsolutePath ChangelogFile => TryGetValue(() => ChangelogFile);

    Target PublishGitHubRelease => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .OnlyWhenStatic(() => IsServerBuild)
        .DependsOn<IUnityPackageVersionMatchesGitTagVersion>()
        .DependsOn<IChangelogVersionMatchesGitTagVersion>()
        .Executes(async () =>
        {
            Assert.True(ChangelogFile != null, "No path has been provided!");

            Changelog changelog = Changelog.FromFile(ChangelogFile);

            var changelogBody = new StringBuilder();

            foreach (var entry in changelog.Sections.First().Entries)
            {
                if (entry.Value.Count == 0)
                    continue;
                
                changelogBody.AppendLine($"### {entry.Key.ToString()}");
                changelogBody.AppendLine();

                foreach (string s in entry.Value)
                {
                    changelogBody.AppendLine($"- {s}");
                }

                changelogBody.AppendLine();
            }

            SemanticVersion version = GitRepository.GetLatestVersionTagOnCurrentCommit();

            var releaseDraft = new NewRelease($"v{version}")
            {
                Draft = true,
                Name = $"v{version}",
                Prerelease = version.IsPrerelease,
                Body = changelogBody.ToString()
            };

            string owner = GitRepository.GetGitHubOwner();
            string name = GitRepository.GetGitHubName();

            var credentials = new Credentials(GitHubActions.Instance.Token);
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new InMemoryCredentialStore(credentials));

            Log.Information("Creating GitHub release...");

            Release release = await GitHubTasks.GitHubClient.Repository.Release.Create(owner, name, releaseDraft);

            await GitHubTasks.GitHubClient.Repository.Release.Edit(owner, name, release.Id, new ReleaseUpdate { Draft = false });
        });
}