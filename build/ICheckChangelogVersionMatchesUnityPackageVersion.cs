using Nuke.Common;
using Serilog;

[ParameterPrefix(nameof(CheckChangelogVersionMatchesUnityPackageVersion))]
interface ICheckChangelogVersionMatchesUnityPackageVersion : INukeBuild
{
    Target CheckChangelogVersionMatchesUnityPackageVersion => _ => _
        .Executes(() =>
        {
            Log.Warning("TODO");
        });
}