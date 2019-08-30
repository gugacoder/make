using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Linq.Entities;
using Microsoft.Deployment.WindowsInstaller.Package;

namespace Do.PlugIns.Msi
{
  class ShowInfoAction : IAction
  {
    public bool Exec(MsiCommandOptions options)
    {
      if (options.Operands.Items.Length > 2)
        return Prompt.PrintInvalidUsage("Argumento não esperado: " + options.Operands.Items.ElementAtOrDefault(2));

      var msiFile = options.Operands.Items.ElementAtOrDefault(1);
      if (msiFile == null)
        return Prompt.PrintInvalidUsage("O arquivo MSI para processamento não foi indicado.");
      
      using (var database = new QDatabase(msiFile, DatabaseOpenMode.ReadOnly))
      {
        Prompt.PrintInfo(msiFile);

        var properties = (
          from prop in database.SummaryInfo.GetType().GetProperties()
          select new
          {
            Property = prop.Name,
            Value = prop.GetValue(database.SummaryInfo, null).ToStringOrNull()
          }
        ).Concat(
          from prop in database.Properties.AsEnumerable()
          select new
          {
            prop.Property,
            prop.Value
          }
        ).ToArray();

        var length = (
          from prop in properties
          let len = prop.Property.Length
          select len
        ).ToArray().DefaultIfEmpty().Max();

        foreach (var prop in properties)
        {
          Prompt.WriteInfo(prop.Property.PadRight(length));
          Prompt.WriteInfo(" = ");
          Prompt.WriteInfoLine(prop.Value);
        }
      }

      return true;
    }
  }
}
