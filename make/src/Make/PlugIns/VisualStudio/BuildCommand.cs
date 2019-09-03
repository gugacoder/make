using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Make.Library.Posix;
using Make.Library.Subversion;
using Make.Library.Shell;
using Make.Library.PackDm;
using System.Text.RegularExpressions;
using Make.Library.Projects;

namespace Make.PlugIns.VisualStudio
{
  class BuildCommand : ICommand<BuildCommandOptions>
  {
    public bool Exec(BuildCommandOptions options)
    {
      Toolchain.RequireDevenv();
      Toolchain.RequirePackDm();
      Toolchain.RequireSubversion();

      bool ok;

      string[] solutions;

      if (options.Solutions.On)
        solutions = options.Solutions.Items.FindFiles();
      else
        solutions = "*.sln".FindFiles();

      if (!solutions.Any())
        return Prompt.PrintNothingToBeDone("Nenhuma solução detectada.");

      var pack = new Library.PackDm.PackDm();
      pack.PackConf = options.ConfFile.Text;
      pack.PackInfo = options.PackFile.Text;

      var svn = new Svn();

      ok = pack.Fill();
      if (!ok) return Prompt.PrintCannotContinue();

      ok = svn.Fill();
      if (!ok) return Prompt.PrintCannotContinue();

      // Imprimindo informação de revisao do pack.info e do subverison
      // no console e no arquivo REVISION.txt
      VersionInfo version;
      {
        var revisionFilepath = Path.GetFullPath("REVISION.txt");

        var loader = new ProjectLoader();
        loader.PackConf = options.ConfFile.Text;
        loader.PackInfo = options.PackFile.Text;
        var ret = loader.LoadProjectHeader();
        if (!ret.Ok) return Prompt.PrintCannotContinue();

        var project = ret.Value;
        version = project.Version;

        // imprimindo no console e no arquivo
        Prompt.PrintInfo(version);
        File.WriteAllText(revisionFilepath, version);
      }

      var configuration = options.Configuration.Text ?? "Release|Any CPU";
      foreach (var solution in solutions)
      {
        var solutionFilename = Path.GetFileName(solution);
        var solutionFolder = Path.GetDirectoryName(solution);

        ok = $"devenv.com {solution.Quote()} /Rebuild {configuration.Quote()}".Run();
        if (!ok) return Prompt.PrintCannotContinue();
      }

      // O packDm usa a máscara para substituir partes da versao
      var versionMask = $"*.*.*{version.FormattedPreRelease}";

      ok = pack.Pack(versionMask);
      if (!ok) return Prompt.PrintCannotContinue();

      // Constrói a página HTML com informações de setup.
      // Falhas podem ser ignoradas.
      var title = Path.GetFileName(solutions.First()).Replace(".sln", "");
      MakeSetupPage(title, version);

      return true;
    }

    /// <summary>
    /// Produz uma página personalizada para o GO.CD com os links de
    /// download dos arquivos de setup e desinstalação.
    /// </summary>
    /// <param name="title">O título do aplicativo.</param>
    /// <param name="revision">O número de revisão.</param>
    private void MakeSetupPage(string title, string revision)
    {
      var dir = @"Dist\Setup";

      if (!Directory.Exists(dir))
        return;

      var files = Directory.GetFiles(dir, "Setup.msi")
          .Concat(Directory.GetFiles(dir, "Setup.*.msi"))
          .Concat(Directory.GetFiles(dir, "Uninstall.bat"))
          .Concat(Directory.GetFiles(dir, "Uninstall.*.bat"))
          .OrderBy(
            file => Path
              .GetFileNameWithoutExtension(file)
              .Replace("Setup", "a")
              .Replace("Uninstall", "a")
          );

      // Monta uma lista contendo o nome original de cada arquivo mais
      // um nome virtual para download, contendo o número de revisão.
      //
      var regex = new Regex(@"^(\w+)(\..*)?\.(\w+)$");
      var links = files.Select(file =>
      {
        // O arquivo pode seguir dois padrões:
        // - Setup.msi
        // ou
        // - Setup.VARIACAO.msi

        var fileName = Path.GetFileName(file);

        var match = regex.Match(fileName);

        var filePrefix = match.Groups[1].Value;
        var fileVariation = match.Groups[2].Value;
        var fileExtension = match.Groups[3].Value;

        // O caminho "../setup" corresponde a um caminho relativo
        // no site do GO.CD.
        var href = $"../setup/{fileName}";

        // Caminho sugerido para o aquivo baixado
        var name = $"{title}{fileVariation}-{revision}-{filePrefix}.{fileExtension}";

        var description =
          fileName.Contains("Uninstall")
            ? $"Script de desinstalação." : $"Pacote de instalação.";

        return new
        {
          href,
          name,
          description
        };
      });

      var html = TextTemplate.ApplyTemplate(
        "Setup.html.template"
        , "title", title
        , "revision", revision
        , "links", links
      );

      var outputFile = Path.GetFullPath(Path.Combine("Dist", "Setup", "go.cd-index.html"));
      var outputDir = Path.GetDirectoryName(outputFile);

      if (!Directory.Exists(outputDir))
      {
        Directory.CreateDirectory(outputDir);
      }

      File.WriteAllText(outputFile, html);
    }
  }
}
