﻿using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Make.Library.Posix
{
  /// <summary>
  /// Atributo opcional para marcação de um argumento como operando.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
  public class OperandAttribute : Attribute
  {
    public readonly string Name;
    public OperandAttribute(params string[] operands)
    {
      Name = string.Join(" ", operands);
    }
  }
}
