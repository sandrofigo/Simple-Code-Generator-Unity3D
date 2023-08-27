using System.IO;
using Newtonsoft.Json;
using NuGet.Versioning;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Serilog;

[ParameterPrefix(nameof(UnityPackageVersionMatchesGitTagVersion))]
interface IUnityPackageVersionMatchesGitTagVersion : INukeBuild
{
    [GitRepository] private GitRepository GitRepository => TryGetValue(() => GitRepository);

    [Parameter] AbsolutePath UnityPackageFile => TryGetValue(() => UnityPackageFile);

    Target UnityPackageVersionMatchesGitTagVersion => _ => _
        .OnlyWhenStatic(() => GitRepository.CurrentCommitHasVersionTag())
        .Executes(() =>
        {
            SemanticVersion versionFromUnityPackageFile = GetVersionFromUnityPackageFile(UnityPackageFile);
            SemanticVersion versionFromGitTag = GitRepository.GetLatestVersionTagOnCurrentCommit();

            Assert.True(versionFromUnityPackageFile == versionFromGitTag, $"The version {versionFromUnityPackageFile} from the Unity package file does not match the version {versionFromGitTag} from the git tag!");
            
            Log.Information("Unity package version and git tag version match");
        });

    static SemanticVersion GetVersionFromUnityPackageFile(AbsolutePath pathToUnityPackageFile)
    {
        Assert.True(pathToUnityPackageFile != null, "No path has been provided!");
        
        dynamic packageFile = JsonConvert.DeserializeObject(File.ReadAllText(pathToUnityPackageFile));
        return SemanticVersion.Parse(packageFile.version.ToString());
    }
}