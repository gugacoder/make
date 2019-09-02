using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace PackDm.Model
{
  [AttributeUsage(AttributeTargets.Property)]
  public class ActionsAttribute : Attribute
  {
    public string[] Names { get; set; }

    public ActionsAttribute(params string[] actions)
    {
      this.Names = actions;
    }
  }
}