using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PackDm.Model
{
  public class Artifact : IComparable
  {
    public Artifact()
    {
    }

    public string Id
    {
      get { return string.Join("/", new string[] { Group, Name, Version }); }
      set
      {
        if (value != null)
        {
          var tokens = value.Split('/');
          Group = tokens.FirstOrDefault();
          Name = tokens.Skip(1).FirstOrDefault();
          Version = tokens.Skip(2).FirstOrDefault() ?? "*";
        }
        else
        {
          Group = null;
          Name = null;
          Version = null;
        }
      }
    }

    public string Group
    {
      get;
      set;
    }

    public string Name
    {
      get;
      set;
    }

    public string Namespace
    {
      get { return string.Join("/", new string[] { Group, Name}); }
    }

    public Tag Version
    {
      get;
      set;
    }

    public override string ToString()
    {
      return Id;
    }

    public bool HasSameRadical(Artifact other)
    {
      return this.Group == other.Group && this.Name == other.Name;
    }

    public override bool Equals(object obj)
    {
      if (obj == null) return false;

      string thisString = this.Id;
      string thatString = obj.ToString();
      return thisString.Equals(thatString);
    }

    public override int GetHashCode()
    {
      return Id.GetHashCode();
    }

    public static implicit operator Artifact(string id)
    {
      return new Artifact{ Id = id };
    }

    public static implicit operator string(Artifact artifact)
    {
      return artifact.ToString();
    }

    public int CompareTo(object obj)
    {
      int comparison = 0;
      var other = (Artifact)obj;

      comparison = Group.CompareTo(other.Group);
      if (comparison != 0) return comparison;

      comparison = Name.CompareTo(other.Name);
      if (comparison != 0) return comparison;

      comparison = Version.CompareTo(other.Version);
      return comparison;
    }
  }
}

