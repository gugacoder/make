using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Make.Library.Posix
{
  /// <summary>
  /// Atributo opcional para personalização de opção.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class OptAttribute : Attribute
  {
    public readonly char? ShortName;
    public readonly string LongName;
    public string Category;
    public OptAttribute() { }
    public OptAttribute(char shortName, string longName) { ShortName = shortName; LongName = longName; }
    public OptAttribute(char shortName) { ShortName = shortName; }
    public OptAttribute(string longName) { LongName = longName; }
  }
}