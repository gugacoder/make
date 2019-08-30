using System;
using System.Linq;
using System.Collections.Generic;
using PackDm.Model;
using System.Text.RegularExpressions;

namespace PackDm.Helpers
{
  public class Semver
  {
    public string UpgradePattern(string pattern, Tag minVersion, Tag maxVersion)
    {
      var olderTags = pattern
        .Split(new[] { " - " }, StringSplitOptions.None)
        .Select(x => (Tag)x)
        .ToArray();

      var newerTags = (olderTags.Length > 1)
        ? new[] { minVersion ?? maxVersion, minVersion ?? maxVersion }
        : new[] { maxVersion ?? minVersion };

      // extrai os numeros da string
      var getnum = new Func<string, int>(value =>
      {
        var digits = Regex.Replace(value ?? "", @"[^\d]", "");
        int.TryParse(digits, out int number);
        return number;
      });

      // remove os numeros da string
      var radical = new Func<string, string>(value =>
        Regex.Replace(value ?? "", @"[^\w]", "")
      );

      // substitui os numeros da string por outro valor
      var replaceNumbers = new Func<string, int, string>((token, num) =>
      {
        var value =
          (token == null || token == "x" || token == "*")
            ? num.ToString()
            : Regex.Replace(token, @"\d+", num.ToString());
        return value;
      });

      for (int i = 0; i < olderTags.Length; i++)
      {
        var oldTag = olderTags[i];
        var newTag = newerTags[i];

        if (newTag == null)
          continue;

        var oldMajor = getnum(oldTag.Major);
        var oldMinor = getnum(oldTag.Minor);
        var oldPatch = getnum(oldTag.Patch);
        var newMajor = getnum(newTag.Major);
        var newMinor = getnum(newTag.Minor);
        var newPatch = getnum(newTag.Patch);

        if (!oldTag.HasMajor || newMajor > oldMajor)
        {
          oldTag.Major = replaceNumbers(oldTag.Major, newMajor);
          oldTag.Minor = replaceNumbers(oldTag.Minor, newMinor);
          oldTag.Patch = replaceNumbers(oldTag.Patch, newPatch);
        }
        else if (newMajor < oldMajor)
        {
          continue;
        }
        else if (!oldTag.HasMinor || newMinor > oldMinor)
        {
          oldTag.Minor = replaceNumbers(oldTag.Minor, newMinor);
          oldTag.Patch = replaceNumbers(oldTag.Patch, newPatch);
        }
        else if (newMinor < oldMinor)
        {
          continue;
        }
        else if (!oldTag.HasPatch || newPatch > oldPatch)
        {
          oldTag.Patch = replaceNumbers(oldTag.Patch, newPatch);
        }
        else if (newPatch < oldPatch)
        {
          continue;
        }
        else if (newTag.HasPreRelease && oldTag.HasPreRelease
          && radical(newTag.PreRelease) == radical(oldTag.PreRelease)
          && getnum(newTag.PreRelease) > getnum(oldTag.PreRelease))
        {
          oldTag.PreRelease = newTag.PreRelease;
        }
      }

      pattern = string.Join(" - ", olderTags.Select(x => x.ToString()));
      return pattern;
    }

    public Tag FindBestMatch(IEnumerable<Tag> versions, string pattern)
    {
      var matches = FindMatches(versions, pattern);
      return matches.LastOrDefault();
    }

    public IEnumerable<Tag> FindMatches(IEnumerable<Tag> versions, string pattern)
    {
      Tag min = null;
      Tag max = null;
      string preReleaseRegex = null;
      
      CreateRange(pattern, out min, out max, out preReleaseRegex);

      Func<Tag, bool> isPreReleaseMatch;
      if (string.IsNullOrWhiteSpace(preReleaseRegex))
      {
        isPreReleaseMatch = tag => true;
      }
      else
      {
        var regex = new Regex(preReleaseRegex);
        isPreReleaseMatch = tag => tag.PreRelease == null || regex.IsMatch(tag.PreRelease);
      }

      return
        from version in versions
        where IsInRange(version, min, max)
           && isPreReleaseMatch(version)
        orderby version
        select version;
    }

    private bool IsInRange(Tag tag, Tag min, Tag max)
    {
      return (tag.CompareTo(min) >= 0) && (tag.CompareTo(max) <= 0);
    }

    private void CreateRange(string pattern, out Tag minumum, out Tag maximum, out string preReleaseRegex)
    {
      Tag tag = null;
      Tag min = null;
      Tag max = null;
      string relation = null;

      if (Regex.IsMatch(pattern, ".* - .*"))
      {
        var tokens = pattern.Split('-').Select(x => x.Trim());
        min = tokens.First();
        max = tokens.Last();
      }
      else
      {
        if (Regex.IsMatch(pattern, "^[=<>^~]"))
        {
          relation = pattern.Substring(0, 1);
          pattern = pattern.Substring(1);
        }
        if (pattern.StartsWith("="))
        {
          relation += pattern.Substring(0, 1);
          pattern = pattern.Substring(1);
        }

        tag = pattern;
        min = pattern;
        max = pattern;
      }

      if (!min.HasMajor || min.Major == "x" || min.Major == "*") min.MajorInt32 = 0;
      if (!min.HasMinor || min.Minor == "x" || min.Minor == "*") min.MinorInt32 = 0;
      if (!min.HasPatch || min.Patch == "x" || min.Patch == "*") min.PatchInt32 = 0;

      if (!max.HasMajor || max.Major == "x" || max.Major == "*") max.MajorInt32 = int.MaxValue;
      if (!max.HasMinor || max.Minor == "x" || max.Minor == "*") max.MinorInt32 = int.MaxValue;
      if (!max.HasPatch || max.Patch == "x" || max.Patch == "*") max.PatchInt32 = int.MaxValue;

      switch (relation)
      {
        case "~":
          {
            max.PatchInt32 = int.MaxValue;
            break;
          }

        case "^":
          {
            if (tag.HasMajor && tag.MajorInt32 > 0)
            {
              max.MinorInt32 = int.MaxValue;
              max.PatchInt32 = int.MaxValue;
            }
            if (tag.HasMinor && tag.MinorInt32 > 0)
            {
              max.PatchInt32 = int.MaxValue;
            }
            break;
          }

        case "<=":
          {
            min = "0.0.0";
            break;
          }

        case ">=":
          {
            max = new Tag
            { 
              MajorInt32 = int.MaxValue,
              MinorInt32 = int.MaxValue,
              PatchInt32 = int.MaxValue 
            };
            break;
          }

        case "<":
          {
            min = "0.0.0";
            if (tag.HasPatch && tag.PatchInt32 > 0)
            {
              max.PatchInt32--;
            }
            else if (tag.HasMinor && tag.MinorInt32 > 0)
            {
              max.MinorInt32--;
              max.PatchInt32 = int.MaxValue;
            }
            else if (tag.HasMajor && tag.MajorInt32 > 0)
            {
              max.MajorInt32--;
              max.MinorInt32 = int.MaxValue;
              max.PatchInt32 = int.MaxValue;
            }
            break;
          }

        case ">":
          {
            max = new Tag
            { 
              MajorInt32 = int.MaxValue,
              MinorInt32 = int.MaxValue,
              PatchInt32 = int.MaxValue 
            };
            if (tag.HasPatch)
            {
              min.PatchInt32++;
            }
            else if (tag.HasMinor)
            {
              min.MinorInt32++;
              min.PatchInt32 = 0;
            }
            else if (tag.HasMajor)
            {
              min.MajorInt32++;
              min.MinorInt32 = 0;
              min.PatchInt32 = 0;
            }
            break;
          }
      }

      minumum = min;
      maximum = max;
      preReleaseRegex = "^" + (tag?.PreRelease ?? "").Replace("*", ".*") + "$";
    }
  }
}

