using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Make.Library.SemVer
{
  static class SemVer
  {
    private static readonly Regex regex = new Regex("^([0-9]+)(?:[.]([0-9]+)(?:[.]([0-9]+)(?:[-](.+))?)?)?$");

    public static string IncreaseMinor(string version)
    {
      var tokens = version.Split('.');
      tokens[2] = (int.Parse(tokens[2]) + 1).ToString();
      return string.Join(".", tokens);
    }

    public static string GetMaxVersion(string[] versions)
    {
      var revs = GetRev(versions);
      var max = revs.DefaultIfEmpty().Max();
      return (max != null) ? max.Version : null;
    }

    public static Rev GetRev(string version)
    {
      var match = regex.Match(version);
      if (!match.Success)
        return null;

      var rev = new Rev
      {
        Version = version,
        Major = ToInt(match.Groups[1].Value),
        Minor = ToInt(match.Groups[2].Value),
        Revision = ToInt(match.Groups[3].Value),
        Variation = match.Groups[4].Value
      };
      return rev;
    }

    public static Rev[] GetRev(IEnumerable<string> versions)
    {
      var revs = versions.Select(GetRev).NonNull().ToArray();
      return revs;
    }

    private static int ToInt(string value)
    {
      int num;
      var ok = int.TryParse(value, out num);
      return ok ? num : 0;
    }

    public class Rev : IComparable<Rev>
    {
      public string Version;
      public int Major;
      public int Minor;
      public int Revision;
      public string Variation;

      public int CompareTo(Rev other)
      {
        int result;

        result = Major.CompareTo(other.Major);
        if (result != 0)
          return result;

        result = Minor.CompareTo(other.Minor);
        if (result != 0)
          return result;

        result = Revision.CompareTo(other.Revision);
        if (result != 0)
          return result;

        result = Variation.CompareTo(other.Variation);
        return result;
      }
    }
  }
}
