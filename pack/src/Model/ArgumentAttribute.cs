using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace PackDm.Model
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ArgumentAttribute : Attribute
  {
    public ArgumentAttribute()
    {
    }

    public ArgumentAttribute(string longName)
    {
      this.LongName = longName;
    }

    public ArgumentAttribute(string longName, string category)
    {
      this.LongName = longName;
      this.Category = category;
    }

    public ArgumentAttribute(char shortName, string longName)
    {
      this.ShortName = shortName;
      this.LongName = longName;
    }

    public ArgumentAttribute(char shortName, string longName, string category)
    {
      this.ShortName = shortName;
      this.LongName = longName;
      this.Category = category;
    }

    public char ShortName
    {
      get;
      set; 
    }

    public string LongName
    {
      get;
      set; 
    }

    public bool IsNameless
    {
      get { return string.IsNullOrWhiteSpace(LongName); }  
    }

    public string Category
    {
      get;
      set; 
    }
  }
}