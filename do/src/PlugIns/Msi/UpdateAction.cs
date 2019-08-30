using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;
using Do.Helpers;
using Do.Library.Msi;
using Do.Library.Projects;
using Do.Library.Shell;
using Do.Library.Subversion;

namespace Do.PlugIns.Msi
{
  class UpdateAction : IAction
  {
    public bool Exec(MsiCommandOptions options)
    {
      // Deve existir pelo menos um nome de arquivo MSI indicado.
      // Mais de um pode ser indicado e o curinga '*' pode ser usado.
      if (options.Operands.Items.Length < 2)
        return Prompt.PrintInvalidUsage("O arquivo MSI para processamento não foi indicado.");

      //
      // Coletando parâmetros
      //

      var msiFilePatterns = options.Operands.Items.Skip(1);
      var msiFiles = Shell.ExpandFileNames(msiFilePatterns);

      if (!msiFiles.Any())
      {
        Prompt.PrintWarn("Arquivo MSI encontrado.");
        return Prompt.PrintInfo("Nada a fazer.");
      }

      var preRelease = options.PreRelease.Text;
      if (preRelease == null && options.InferPreRelease.On)
      {
        var loader = new ProjectLoader();
        var ret = loader.LoadProjectHeader();
        preRelease = ret.Value?.Version?.PreRelease?.Name;
      }

      //
      // Transformando os MSI
      //

      var transformation = new MsiTransformationAlgorithm.Transformation
      {
        PreRelease = preRelease,
        ProductName = options.ProductName,
        ProductTitle = options.ProductTitle,
        ServicePort = options.ServicePort
      };

      foreach (var msiFile in msiFiles)
      {
        var tempFile = Path.GetTempFileName();
        File.Copy(msiFile, tempFile, overwrite: true);

        var ok = MsiTransformationAlgorithm.ApplyTransformation(tempFile, transformation);
        if (ok)
        {
          File.Copy(tempFile, msiFile, overwrite: true);
        }

        File.Delete(tempFile);
      }

      return Prompt.PrintInfo($"Feito!");
    }
  }
}
