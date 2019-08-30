using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Do.Library.Posix
{
  /// <summary>
  /// Atributo opcional para personalização da descrição de uso.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class HelpUsageAttribute : HelpAttribute
  {
    public HelpUsageAttribute(params string[] lines) : base(lines) { }
  }
}
