using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace PackDm.Model
{
  [AttributeUsage(AttributeTargets.Property)]
  public class DescriptionAttribute : Attribute
  {
    public string[] TextLines { get; set; }

    public DescriptionAttribute(params string[] textLines)
    {
      this.TextLines = textLines;
    }
  }
}