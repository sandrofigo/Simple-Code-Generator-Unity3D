// Copyright 2024 © Sandro Figo

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Nuke.Common.IO;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

public class Changelog
{
    public ChangelogTextSection Header;
    public List<ChangelogReleaseSection> Sections = new();

    public static Changelog FromFile(AbsolutePath changelogFile)
    {
        string[] lines = changelogFile.ReadAllLines();

        return new Changelog
        {
            Header = GetHeader(lines),
            Sections = GetSections(lines)
        };
    }

    static bool IsSectionStart(string line)
    {
        return line.StartsWith("## ");
    }

    static bool IsSubSectionStart(string line)
    {
        return line.StartsWith("### ");
    }

    static bool IsSubSectionEntry(string line)
    {
        return line.StartsWith("-");
    }

    static bool IsVersionComparison(string line)
    {
        return line.StartsWith("[");
    }

    static ChangelogTextSection GetHeader(IEnumerable<string> lines)
    {
        var header = new ChangelogTextSection();

        foreach (string l in lines)
        {
            if (IsSectionStart(l))
            {
                return header;
            }

            header.Lines.Add(l);
        }

        return header;
    }


    static List<ChangelogReleaseSection> GetSections(IEnumerable<string> lines)
    {
        var sections = new List<ChangelogReleaseSection>();

        string currentSubSection = "";

        foreach (string l in lines)
        {
            if (IsSectionStart(l))
            {
                string currentSectionVersion = Regex.Match(l, @"\[(?<version>.*)\]").Groups["version"].Value.Trim();
                string currentSectionReleaseDate = Regex.Match(l, @"(?<release_date>\d+-\d+-\d+)").Groups["release_date"].Value.Trim();

                sections.Add(new ChangelogReleaseSection
                {
                    Version = currentSectionVersion,
                    ReleaseDate = currentSectionReleaseDate
                });

                currentSubSection = "";
            }
            else if (IsSubSectionStart(l) && sections.Count > 0)
            {
                string subSectionName = l.Replace("###", "").Trim().ToLowerInvariant();

                currentSubSection = subSectionName;
            }
            else if (IsSubSectionEntry(l) && !currentSubSection.IsNullOrWhiteSpace())
            {
                string entry = l.TrimStart("-").Trim();

                if (Enum.TryParse(currentSubSection, true, out EntryType entryType))
                {
                    sections[^1].Entries[entryType].Add(entry);
                }
            }
        }

        return sections;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        if (Header != null)
            stringBuilder.AppendLine(Header.ToString());

        stringBuilder.AppendJoin(System.Environment.NewLine, Sections);

        return stringBuilder.ToString();
    }
}

public class ChangelogReleaseSection
{
    public string Version { get; set; }
    public string ReleaseDate { get; set; }

    public Dictionary<EntryType, List<string>> Entries { get; set; } = new()
    {
        { EntryType.Added, new List<string>() },
        { EntryType.Changed, new List<string>() },
        { EntryType.Deprecated, new List<string>() },
        { EntryType.Removed, new List<string>() },
        { EntryType.Fixed, new List<string>() },
        { EntryType.Security, new List<string>() }
    };

    public override string ToString()
    {
        var lines = new List<string>();

        lines.Add($"## [{Version}]{(ReleaseDate.IsNullOrWhiteSpace() ? "" : $" - {ReleaseDate}")}");
        lines.Add("");

        foreach (var group in Entries)
        {
            if (!group.Value.IsEmpty())
            {
                lines.Add($"### {group.Key}");
                lines.Add("");
                foreach (string s in group.Value)
                    lines.Add($"- {s}");
                lines.Add("");
            }
        }

        var stringBuilder = new StringBuilder();

        return stringBuilder.AppendJoin(System.Environment.NewLine, lines).ToString();
    }
}

public class ChangelogTextSection
{
    public List<string> Lines { get; } = new();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendJoin(System.Environment.NewLine, Lines);

        return stringBuilder.ToString();
    }
}

public enum EntryType
{
    Added,
    Changed,
    Deprecated,
    Removed,
    Fixed,
    Security,
}