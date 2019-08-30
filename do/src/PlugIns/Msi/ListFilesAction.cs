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
  class ListFilesAction : IAction
  {
    public bool Exec(MsiCommandOptions options)
    {
      if (options.Operands.Items.Length > 2)
        return Prompt.PrintInvalidUsage("Argumento não esperado: " + options.Operands.Items.ElementAtOrDefault(2));

      var msiFile = options.Operands.Items.ElementAtOrDefault(1);
      if (msiFile == null)
        return Prompt.PrintInvalidUsage("O arquivo MSI para processamento não foi indicado.");

      using (var package = new InstallPackage(msiFile, DatabaseOpenMode.ReadOnly))
      {
        Prompt.PrintInfo(msiFile);

        var files =
          from file in package.Files.AsEnumerable()
          orderby file.Value.TargetName
          select file.Value.TargetName;

        string previous = null;
        foreach (var file in files)
        {
          Prompt.WriteInfo(file);

          if (file == previous)
          {
            Prompt.WriteInfo(" (DUPLICADO)");
          }

          Prompt.WriteInfoLine("");

          previous = file;
        }
      }

      return true;
    }
  }
}
