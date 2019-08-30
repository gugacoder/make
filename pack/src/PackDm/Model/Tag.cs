using System;
using System.Linq;
using System.Collections.Generic;

namespace PackDm.Model
{
  public class Tag : IComparable<Tag>
  {
    public Tag()
    {
    }

    public string Version
    {
      get
      {
        var number = string.Join(".", new[] { Major, Minor, Patch }.Where(x => x != null));
        if (!string.IsNullOrWhiteSpace(PreRelease))
          number += "-" + PreRelease;
        return number;
      }
      set
      {
        if (value == null)
        {
          this.Major = null;
          this.Minor = null;
          this.Patch = null;
          this.PreRelease = null;
          return;
        }

        var tokens = value.Split('-');
        this.PreRelease = tokens.Skip(1).FirstOrDefault();

        var numbers = tokens.First();
        tokens = numbers.Split('.');

        this.Major = tokens.FirstOrDefault();
        this.Minor = tokens.Skip(1).FirstOrDefault();
        this.Patch = tokens.Skip(2).FirstOrDefault();
      }
    }

    public string Major
    {
      get;
      set;
    }

    public string Minor
    {
      get;
      set;
    }

    public string Patch
    {
      get;
      set;
    }

    public string PreRelease
    {
      get;
      set;
    }

    public bool HasMajor
    {
      get { return MajorInt32 > -1; }
    }

    public bool HasMinor
    {
      get { return MinorInt32 > -1; }
    }

    public bool HasPatch
    {
      get { return PatchInt32 > -1; }
    }

    public bool HasPreRelease
    {
      get { return PreRelease != null; }
    }

    public int MajorInt32
    {
      get
      {
        return int.TryParse(Major, out int number) ? number : -1;
      }
      set { Major = value.ToString(); }
    }

    public int MinorInt32
    {
      get
      {
        return int.TryParse(Minor, out int number) ? number : -1;
      }
      set { Minor = value.ToString(); }
    }

    public int PatchInt32
    {
      get
      {
        return int.TryParse(Patch, out int number) ? number : -1;
      }
      set { Patch = value.ToString(); }
    }

    public bool IsValid
    {
      get { return HasMajor && HasMinor && HasPatch; }
    }

    #region IComparable implementation

    public int CompareTo(Tag tag)
    {
      int value = 0;

      value = MajorInt32.CompareTo(tag.MajorInt32);
      if (value != 0)
        return value;

      value = MinorInt32.CompareTo(tag.MinorInt32);
      if (value != 0)
        return value;

      value = PatchInt32.CompareTo(tag.PatchInt32);
      if (value != 0)
        return value;

      if (PreRelease == null && tag.PreRelease == null)
        return 0;

      if (PreRelease == null)
        return -1;

      if (tag.PreRelease == null)
        return 1;

      value = PreRelease.CompareTo(tag.PreRelease);
      return value;
    }

    #endregion

    public static IEnumerable<Tag> SelectValidTags(IEnumerable<Tag> tags)
    {
      return tags.Where(t => t.IsValid);
    }

    public override string ToString()
    {
      return Version;
    }

    public static implicit operator Tag(string version)
    {
      return new Tag{ Version = version ?? "0.0.0" };
    }

    public static implicit operator string(Tag version)
    {
      return (version != null) ? version.ToString() : "0.0.0";
    }
  }
}

