using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Do
{
  [AttributeUsage(AttributeTargets.Class)]
  class CommandAttribute : ExportAttribute
  {
    public new const string ContractName = "Command";

    public CommandAttribute()
      : base(ContractName)
    {
    }
  }
}
