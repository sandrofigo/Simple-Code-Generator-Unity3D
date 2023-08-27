// #define MOCK_GIT_TAG

using System.Linq;
using NuGet.Versioning;
using Nuke.Common.Git;
using Nuke.Common.Utilities.Collections;

public static class GitRepositoryExtensions
{
    public static bool CurrentCommitHasVersionTag(this GitRepository gitRepository)
    {
#if MOCK_GIT_TAG
        return true;
#endif

        var versionTagsOnCurrentCommit = gitRepository.Tags.Select(t => SemanticVersion.TryParse(t.TrimStart('v'), out SemanticVersion v) ? v : null).WhereNotNull();

        return versionTagsOnCurrentCommit.Any();
    }

    public static SemanticVersion GetLatestVersionTagOnCurrentCommit(this GitRepository gitRepository)
    {
#if MOCK_GIT_TAG
        return new SemanticVersion(1, 0, 0);
#endif

        var versionTagsOnCurrentCommit = gitRepository.Tags.Select(t => SemanticVersion.TryParse(t.TrimStart('v'), out SemanticVersion v) ? v : null).WhereNotNull().OrderByDescending(t => t).ToArray();

        return versionTagsOnCurrentCommit.Any() ? versionTagsOnCurrentCommit.First() : new SemanticVersion(0, 0, 0);
    }
}