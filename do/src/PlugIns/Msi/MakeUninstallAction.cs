using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Do.Helpers;
using Do.Library.Shell;
using Do.Library.Msi;

namespace Do.PlugIns.Msi
{
  class MakeUninstallAction : IAction
  {
    public bool Exec(MsiCommandOptions options)
    {
      Prompt.PrintInfo("Criando o desinstalador...");

      //
      // Validando parametros
      //

      // Deve existir pelo menos um nome de arquivo MSI indicado.
      // Mais de um pode ser indicado e o curinga '*' pode ser usado.
      if (options.Operands.Items.Length < 2)
        return Prompt.PrintInvalidUsage("O arquivo MSI para processamento não foi indicado.");

      var msiFilePatterns = options.Operands.Items.Skip(1);
      var msiFiles = Shell.ExpandFileNames(msiFilePatterns);

      foreach (var msiFile in msiFiles)
      {
        //
        // Definindo o nome do arquivo destino
        //
        var filepath = GetValidUninstallScriptFilePath(msiFile, options);

        //
        // Extraindo parametros do arquivo MSI
        //
        string productName;
        string productCode;
        string serviceName;

        using (var msi = new MsiDatabase(msiFile))
        {
          productName = msi.ProductName;
          productCode = msi.ProductCode;
          serviceName = msi.ProductName;
        }

        Prompt.PrintInfo($"ProductName={productName}");
        Prompt.PrintInfo($"ProductCode={productCode}");
        Prompt.PrintInfo($"ServiceName={serviceName}");

        CreateUninstallBatch(msiFile, filepath, productName, productCode, serviceName);

        Prompt.PrintInfo($"Script gerado:\n  {filepath}");
      }

      return true;
    }

    public static void CreateUninstallBatch(string msiFile, string targetFile, string productName, string productCode, string serviceName)
    {
      //
      // Produzindo o batch de desinstação baseado no template embarcado
      //
      var template = EmbeddedFiles.RetrieveTextFile("Uninstall.bat.template");
      var text =
        template
          .Replace("${ProductName}", productName)
          .Replace("${ProductCode}", productCode)
          .Replace("${ServiceName}", serviceName);

      File.WriteAllText(targetFile, text);
    }

    public static string GetValidUninstallScriptFilePath(string msiFile, MsiCommandOptions options)
    {
      var msiDir = Path.GetDirectoryName(msiFile);

      var dirname = (options.OutputDir?.On == true) ? options.OutputDir.Text : msiDir;
      var filename =
        (options.OutputFile?.On == true)
          ? options.OutputFile.Text
          : InferUninstallScriptFilename(msiFile);

      string filepath = Path.GetFullPath(Path.Combine(dirname, filename));
      string dirpath = Path.GetDirectoryName(filepath);

      if (!Directory.Exists(dirpath))
        Directory.CreateDirectory(dirpath);

      return filepath;
    }

    public static string InferUninstallScriptFilename(string msiFile)
    {
      var filename = Path.GetFileNameWithoutExtension(msiFile);
      if (filename.Contains("Setup"))
      {
        filename = filename.Replace("Setup", "Uninstall");
      }
      else if (filename.Contains("setup"))
      {
        filename = filename.Replace("setup", "uninstall");
      }
      else
      {
        if (filename == "" || filename.EndsWith("-"))
          filename += "Uninstall";
        else
          filename += "-Uninstall";
      }
      filename += ".bat";
      return filename;
    }
  }
}
