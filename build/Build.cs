using System;
using System.Linq;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Serilog;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

[GitHubActions("tests",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "main" },
    InvokedTargets = new[] { nameof(Test) },
    PublishCondition = "always()",
    EnableGitHubToken = true,
    JobConcurrencyCancelInProgress = true
)]
[GitHubActions("publish",
    GitHubActionsImage.UbuntuLatest,
    InvokedTargets = new[] { nameof(Publish) },
    PublishCondition = "always()",
    EnableGitHubToken = true,
    OnPushTags = new[] { "v[0-9]+.[0-9]+.[0-9]+" }
)]
class Build : NukeBuild, ICheckForUnityMetaFiles, IUnityPackageVersionMatchesGitTagVersion, IChangelogVersionMatchesGitTagVersion, IPublishGitHubRelease
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution]
    readonly Solution UnitTestProject;

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetTasks.DotNetClean(s => s
                .SetProject(UnitTestProject));
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetTasks.DotNetRestore(settings => settings
                .SetProjectFile(UnitTestProject)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .DependsOn<ICheckForUnityMetaFiles>()
        .Executes(() =>
        {
            DotNetTasks.DotNetBuild(settings => settings
                .SetProjectFile(UnitTestProject)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
            );
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(RootDirectory / "tests" / "TestResults" / "*.trx")
        .Executes(() =>
        {
            DotNetTasks.DotNetTest(settings => settings
                .SetProjectFile(UnitTestProject)
                .SetConfiguration(Configuration)
                .SetLoggers("trx;logfilename=test-results.trx")
                .EnableNoRestore()
                .EnableNoBuild()
                .AddLoggers("GitHubActions")
            );
        });

    Target Publish => _ => _
        .DependsOn(Test)
        .Triggers<IPublishGitHubRelease>();
}