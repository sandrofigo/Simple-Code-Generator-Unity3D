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
class Build : NukeBuild, IUnityPackageVersionMatchesGitTagVersion, IChangelogVersionMatchesGitTagVersion, IPublishGitHubRelease
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
        .DependsOn(CheckForUnityMetaFiles)
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

    Target CheckForUnityMetaFiles => _ => _
        .Executes(() =>
        {
            // Verify that all files and directories have a Unity meta file (if the meta file is missing the file will be ignored when imported into Unity)
            AssertThatUnityMetaFilesExist([RootDirectory / "src" / "Assets" / "SimpleCodeGenerator"], []);
        });

    /// <summary>
    /// Checks recursively if all files and folders have a Unity meta file.
    /// </summary>
    static void AssertThatUnityMetaFilesExist(AbsolutePath[] includeDirectories, AbsolutePath[] excludeDirectories)
    {
        excludeDirectories ??= [];

        if (includeDirectories.IsNullOrEmpty())
            Assert.Fail("No directories have been provided to check for .meta files!");

        Log.Information("Checking if all necessary Unity .meta files exist...");

        int totalDirectoriesChecked = 0;
        int totalFilesChecked = 0;

        foreach (AbsolutePath includeDirectory in includeDirectories)
        {
            var directories = includeDirectory.GlobDirectories("**").Where(d => d != includeDirectory);

            foreach (AbsolutePath d in directories)
            {
                if (excludeDirectories.Contains(d))
                    continue;

                Assert.True((d.Parent / (d.Name + ".meta")).FileExists(), $"The directory '{d}' does not have a Unity meta file!");

                totalDirectoriesChecked++;
            }

            var files = includeDirectory.GlobFiles("**/*").Where(f => !f.ToString().EndsWith(".meta"));

            foreach (AbsolutePath f in files)
            {
                if (excludeDirectories.Contains(f.Parent))
                    continue;

                Assert.True((f.Parent / (f.Name + ".meta")).FileExists(), $"The file '{f}' does not have a Unity meta file!");

                totalFilesChecked++;
            }
        }

        Log.Information("Checked a total of {Directories} directories and {Files} files", totalDirectoriesChecked, totalFilesChecked);
    }
}