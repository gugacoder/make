using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Make.Library.Posix;
using Make.Library.Shell;
using System.Reflection;

namespace Make
{
  /// <summary>
  /// Utilitário para resolução de ferramentas externas.
  /// 
  /// Cada método "Require*()" recupera da melhor forma possível
  /// as depedências da ferramenta correspondente.
  /// 
  /// Binários são geralmente estocados em uma subpasta "packages\do\tools".
  /// </summary>
  static class Toolchain
  {
    private const string ToolsFolder = @"packages\do\tools";

    public static void Initialize()
    {
      AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(
        (o, a) =>
        {
          var assemblyName = a.Name.Split(',').First();
          var filename = assemblyName + ".dll";
          if (EmbeddedFiles.Exists(filename))
          {
            var bytes = EmbeddedFiles.RetrieveBinary(filename);
            return Assembly.Load(bytes);
          }
          return null;
        }
      );
    }

    public static void RequireAll(bool force = false)
    {
      RequireToolsPath();

      var type = typeof(Toolchain);
      var methods =
        from m in type.GetMethods()
        where m.Name.StartsWith("Require")
           && m.Name != "RequireAll"
        select m;

      foreach (var method in methods)
      {
        method.Invoke(null, new object[] { force });
      }
    }

    public static void RequireToolsPath(bool force = false)
    {
      var toolsPath = Path.GetFullPath(ToolsFolder);

      if (!Directory.Exists(toolsPath))
        Directory.CreateDirectory(toolsPath);

      var path = Environment.GetEnvironmentVariable("PATH");
      if (!path.Contains(toolsPath))
      {
        path += ";" + toolsPath;
        Environment.SetEnvironmentVariable("PATH", path);

        Prompt.PrintInfo("Caminho dos binários:\n" + toolsPath);
      }
    }

    public static void RequirePackDm(bool force = false)
    {
      RequireToolsPath();

#if WINDOWS
      var packExe = "pack.exe";
      var bootstrapExe = "pack-bootstrap.exe";
#else
      var packFile = "pack";
      var bootstrapFile = "pack-bootstrap";
#endif

      var bootstrapPath = Path.GetFullPath($@"{ToolsFolder}\{bootstrapExe}");
      if (force || !File.Exists(bootstrapPath))
      {
        using (var output = new FileStream(bootstrapPath, FileMode.Create))
        {
          EmbeddedFiles.CopyTo(bootstrapExe, output);
        }
      }

      var packPath = Path.GetFullPath($@"{ToolsFolder}\{packExe}");
      if (force || !File.Exists(packPath))
      {
        using (Shell.ChDir(ToolsFolder))
        {
          Shell.Run(bootstrapExe);
        }
      }

      Prompt.PrintInfo("PackDm detectado:\n" + bootstrapPath);
    }

    public static void RequireDevenv(bool force = false)
    {
      var programFiles = new[] {
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
        Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
      }.Distinct();

      var folders = new[] {
        @"Microsoft Visual Studio\2019\Community",
        @"Microsoft Visual Studio\2017\Community",
        @"Microsoft Visual Studio 16.0",
        @"Microsoft Visual Studio 15.0",
        @"Microsoft Visual Studio 10.0",
        @"Microsoft Visual Studio 9.0"
      };

      var filepaths =
        from programFile in programFiles
        from folder in folders
        select Path.Combine(programFile, folder, @"Common7\IDE\devenv.com");

      var devenv = (
        from filepath in filepaths
        where File.Exists(filepath)
        select filepath
      ).FirstOrDefault();

      if (devenv == null)
        throw new PromptException("Nenhuma versão compatível do VisualStudio foi detectada.");

      var devenvPath = Path.GetDirectoryName(devenv);
      var path = Environment.GetEnvironmentVariable("PATH");
      if (!path.Contains(devenvPath))
      {
        path += ";" + devenvPath;
        Environment.SetEnvironmentVariable("PATH", path);
      }

      Prompt.PrintInfo($"VisualStudio detectado:\n{devenv}");

      ApplyDevenvWorkaround(devenv);
    }

    public static void RequireSubversion(bool force = false)
    {
      bool ok;

      RequireToolsPath(force);

      ok = "svn --version".Run();
      if (!ok)
      {
        var programFiles = new[] {
          Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
          Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        }.Distinct();

          var folders = new[] {
          @"TortoiseSVN\bin"
        };

        var filepaths =
          from programFile in programFiles
          from folder in folders
          select Path.Combine(programFile, folder, @"svn.exe");

        var svn = (
          from filepath in filepaths
          where File.Exists(filepath)
          select filepath
        ).FirstOrDefault();

        if (svn == null)
          throw new PromptException("Nenhuma versão do Subversion foi detectada.");

        var path = Environment.GetEnvironmentVariable("PATH");
        path += ";" + Path.GetDirectoryName(svn);
        Environment.SetEnvironmentVariable("PATH", path);
      }

      Prompt.PrintInfo($"Subversion detectado:\nsvn");
    }

    public static void RequireNuGet(bool force = false)
    {
      RequireToolsPath();

      var exe = Path.GetFullPath($@"{ToolsFolder}\nuget.exe");

      if (force || !File.Exists(exe))
      {
        using (var output = new FileStream(exe, FileMode.Create))
        {
          EmbeddedFiles.UnpackTo("nuget.exe.gz", output);
        }
      }

      Prompt.PrintInfo("NuGet detectado:\n" + exe);
    }

    private static void ApplyDevenvWorkaround(string devenv)
    {
      // Workaround VisualStudio 2012+
      // 
      // Correção para permitir ao VisualStudio 2017 compilar projetos do tipo
      // Setup na linha de comando.
      // 
      // Leia mais no comentário sobre o Workaround no início do arquivo.

      var folder = Path.GetDirectoryName(devenv);
      var workaround = Path.Combine(folder,
        @"CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild\DisableOutOfProcBuild.exe"
      );

      if (File.Exists(workaround))
      {
        Prompt.PrintInfo(
          "Aplicando um workaround para evitar o erro: HRESULT = '8000000A'\n"
        );

        using (workaround.GetDir().ChDir())
        {
          var ok = workaround.Quote().Run();
          if (!ok) Prompt.PrintWarn("Falhou a aplicação do workaround.");
        }
      }
    }
  }
}