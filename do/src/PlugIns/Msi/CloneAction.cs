using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using Microsoft.Deployment.WindowsInstaller.Linq.Entities;
using Microsoft.Deployment.WindowsInstaller.Package;
using Do.Helpers;
using Do.Library.Msi;
using Do.Library.Projects;
using Do.Library.Shell;

namespace Do.PlugIns.Msi
{
  class CloneAction : IAction
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
          var targetFile = MakeTargetFilePath(msiFile, options, transformation);
          File.Copy(tempFile, targetFile, overwrite: true);
        }

        File.Delete(tempFile);
      }

      return Prompt.PrintInfo($"Feito!");
    }

    /// <summary>
    /// Cria um nome apropriado para o clone do MSI recém criado.
    /// </summary>
    /// <param name="msiFile">O caminho do arquivo MSI original.</param>
    /// <param name="options">As opções de linha de comando.</param>
    /// <param name="transformation">Transformações realizadas.</param>
    /// <returns>O nome de arquivo inventado.</returns>
    private string MakeTargetFilePath(string msiFile, MsiCommandOptions options, MsiTransformationAlgorithm.Transformation transformation)
    {
      var dirpath = options.OutputDir.On ? options.OutputDir.Text : ".";
      if (options.OutputFile.On)
      {
        var filename = options.OutputFile.Text;
        var filepath = Path.GetFullPath(Path.Combine(dirpath, filename));
        return filepath;
      }
      else
      {
        string filename;
        string filepath;

        var radical = Path.GetFileNameWithoutExtension(msiFile);

        radical = Regex.Replace(radical, @"\.clone\d*$", "");

        if (!string.IsNullOrEmpty(transformation.PreRelease))
        {
          var suffix = $"-{transformation.PreRelease.ToUpper()}";
          if (!radical.EndsWith(suffix))
          {
            radical += suffix;

            filename = $"{radical}.msi";
            if (!File.Exists(filename))
            {
              filepath = Path.GetFullPath(Path.Combine(dirpath, filename));
              return filepath;
            }
          }
        }

        int clone = 1;
        do
        {
          filename = $"{radical}.clone{clone++}.msi";
        } while (File.Exists(filename));

        filepath = Path.GetFullPath(Path.Combine(dirpath, filename));
        return filepath;
      }
    }
  }
}
