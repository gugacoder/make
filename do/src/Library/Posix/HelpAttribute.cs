using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Do.Library.Posix
{
  /// <summary>
  /// Atributo opcional para texto de ajuda sobre argumento.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class HelpAttribute : Attribute
  {
    public string Text;
    public HelpAttribute(params string[] lines) { Text = lines.JoinLines(); }
  }
}
