using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Do.Library.Posix;

namespace Do.Library.Shell
{
  static class Shell
  {
    #region Extensões

    public static string Quote(this string text)
    {
      return
        text.Contains(" ") || text.Contains("\n") || text.Contains("\r") || text.Contains("\t")
          ? $"\"{text}\""
          : text;
    }

    #endregion

    #region Comandos básicos

    /// <summary>
    /// Resolve o caracter curinga na definição de nomes produzindo uma lista
    /// dos nomes de arquivo existentes que correspondem ao padrão.
    /// </summary>
    /// <param name="filePatterns">Os padrões de nome com o curinga "*" suportado.</param>
    /// <returns>A relação de nomes de arquivos encontrados.</returns>
    public static IEnumerable<string> ExpandFileNames(IEnumerable<string> filePatterns)
    {
      foreach (var pattern in filePatterns)
      {
        var tokens = pattern.Split('\\');
        var pathPattern = string.Join("\\", tokens.Take(tokens.Length - 1));
        var namePattern = tokens.Last();

        var folders = FindFolders(pathPattern);
        foreach (var folder in folders)
        {
          var filepaths = Directory.GetFiles(folder, namePattern, SearchOption.TopDirectoryOnly);
          foreach (var filepath in filepaths)
          {
            yield return filepath;
          }
        }
      }
    }

    private static IEnumerable<string> FindFolders(string pattern)
    {
      var rootDir = new DirectoryInfo(Directory.GetCurrentDirectory());

      Match match;

      match = Regex.Match(pattern, @"^(\\\\[^\\]+\\[^\\]+\\)(.*)");
      if (match.Success)
      {
        rootDir = new DirectoryInfo(match.Groups[1].Value);
        pattern = match.Groups[2].Value;
      }

      match = Regex.Match(pattern, @"^(.:\\)(.*)");
      if (match.Success)
      {
        rootDir = new DirectoryInfo(match.Groups[1].Value);
        pattern = match.Groups[2].Value;
      }

      var tokens = pattern.Split(@"\");
      IEnumerable<DirectoryInfo> matches = new[] { rootDir };

      foreach (var token in tokens)
      {
        if (token == "..")
        {
          matches = matches.Select(dir => dir.Parent);
        }
        else if (token != "." && token != "")
        { 
          matches = matches.Select(dir => dir.GetDirectories(token)).SelectMany(x => x);
        }
      }
      return matches.Select(x => x.FullName);
    }

    /// <summary>
    /// Acessa uma pasta e retorna para a pasta anterior ao sair do bloco.
    /// </summary>
    /// <param name="folder">A pasta a ser acessada.</param>
    /// <returns>Um objeto para controle do bloco.</returns>
    public static Disposable ChDir(this string folder)
    {
      Prompt.PrintLine();
      Prompt.PrintLine($"[cmd] cd {folder.Quote()}");

      var curdir = Directory.GetCurrentDirectory();
      Directory.SetCurrentDirectory(folder);

      var disposable = new Disposable();
      disposable.Disposed += (o, q) =>
      {
        Prompt.PrintLine();
        Prompt.PrintLine($"[cmd] cd {curdir.Quote()}");
        Directory.SetCurrentDirectory(curdir);
      };
      return disposable;
    }

    /// <summary>
    /// Obtém o diretório corrente.
    /// </summary>
    public static string GetDir(this string path)
    {
      var dir = Path.GetDirectoryName(path);
      return dir == "" ? "." : dir;
    }

    /// <summary>
    /// Cria uma pasta e subpastas.
    /// </summary>
    public static void MkDir(this string path)
    {
      if (!Directory.Exists(path))
      {
        Prompt.PrintLine();
        Prompt.PrintLine($"[cmd] mkdir {path.Quote()}");
        Directory.CreateDirectory(path);
      }
    }

    /// <summary>
    /// Obtém os arquivos que corresponde ao filtro a partir da pasta corrente.
    /// </summary>
    /// <param name="criteria">
    /// O critério de pesquisa.
    /// O caracter curinga `*` pode ser usado.
    /// </param>
    /// <returns>O caminho completo dos arquivos encontrados.</returns>
    public static string[] FindFiles(this string criteria)
    {
      var folder = Path.GetDirectoryName(criteria);
      var filter = Path.GetFileName(criteria);

      if (folder == "") folder = ".";

      var files = Directory.GetFiles(folder, filter);
      files = files.Select(x => Path.GetFullPath(x)).ToArray();
      return files;
    }

    /// <summary>
    /// Obtém os arquivos que corresponde ao filtro a partir da pasta corrente.
    /// O resultado contém os arquivos obtidos pela aplicação de cada filtro individualmente.
    /// </summary>
    /// <param name="criteria">
    /// Os critérios de pesquisa.
    /// O caracter curinga `*` pode ser usado.
    /// </param>
    /// <returns>O caminho completo dos arquivos encontrados.</returns>
    public static string[] FindFiles(this string[] criterias)
    {
      var files = criterias.SelectMany(criteria => FindFiles(criteria)).ToArray();
      return files;
    }

    /// <summary>
    /// Imprime uma mensagem no console ou no arquivo indicado.
    /// </summary>
    /// <param name="text">A mensagem a ser impressa.</param>
    /// <param name="filepath">
    /// Um caminho opcional de arquivo.
    /// Se omitido a mensagem é impressa no console.
    /// </param>
    public static void Echo(this string text, string filepath = null)
    {
      if (filepath != null)
      {
        Prompt.PrintLine();
        Prompt.PrintLine($"[cmd] echo {text} > {filepath.Quote()}");
        MkDir(filepath.GetDir());
        File.WriteAllText(filepath, text);
      }
      else
      {
        Console.WriteLine(text);
      }
    }

    /// <summary>
    /// Grava um texto em um arquivo.
    /// </summary>
    /// <param name="text">O texto a ser gravado.</param>
    /// <param name="filepath">O arquivo desstino.</param>
    public static void Save(this string text, string filepath = null)
    {
      MkDir(filepath.GetDir());
      File.WriteAllText(filepath, text);
    }

    /// <summary>
    /// Lê todo o texto do arquivo.
    /// Se o arquivo não existir nulo é retornado.
    /// </summary>
    /// <param name="filepath">Caminho do arquivo.</param>
    /// <returns>O texto do arquivo ou nulo.</returns>
    public static string Read(this string filepath)
    {
      if (!File.Exists(filepath))
        return null;

      return File.ReadAllText(filepath);
    }

    #endregion

    #region Comandos do shell

    /// <summary>
    /// Executa um comando do shell.
    /// </summary>
    /// <param name="command">O comando a ser executado.</param>
    /// <returns>Verdadeiro se o comando foi bem sucedido.</returns>
    public static bool Run(this string command)
    {
      string output = null;
      string err = null;
      return RunCmd(command, false, false, out output, out err);
    }

    /// <summary>
    /// Executa um comando do shell.
    /// </summary>
    /// <param name="command">O comando a ser executado.</param>
    /// <param name="output">Saída padrão do shell.</param>
    /// <returns>Verdadeiro se o comando foi bem sucedido.</returns>
    public static bool Run(this string command, out string output)
    {
      string err = null;
      return RunCmd(command, true, false, out output, out err);
    }

    /// <summary>
    /// Executa um comando do shell.
    /// </summary>
    /// <param name="command">O comando a ser executado.</param>
    /// <param name="outputError">
    /// Quando verdadeiro emite a saída de erro. Quando falso emite a saída padrão.
    /// </param>
    /// <param name="output">Saída escolhida.</param>
    /// <returns>Verdadeiro se o comando foi bem sucedido.</returns>
    public static bool Run(this string command, bool outputError, out string output)
    {
      string skip = null;
      return outputError
        ? RunCmd(command, false, true, out skip, out output)
        : RunCmd(command, true, false, out output, out skip);
    }

    /// <summary>
    /// Executa um comando do shell.
    /// </summary>
    /// <param name="command">O comando a ser executado.</param>
    /// <param name="output">Saída padrão do shell.</param>
    /// <param name="err">Saída de erro do shell.</param>
    /// <returns>Verdadeiro se o comando foi bem sucedido.</returns>
    public static bool Run(this string command, out string output, out string err)
    {
      return RunCmd(command, true, true, out output, out err);
    }

    /// <summary>
    /// Executa um comando do shell.
    /// </summary>
    /// <param name="command">O comando a ser executado.</param>
    /// <param name="emitStdOutput">Diz se a saída padrão deve ser resultada.</param>
    /// <param name="emitErrOutput">Diz se a saída de erro deve ser resultada.</param>
    /// <param name="stdOutput">A saída padrão.</param>
    /// <param name="errOutput">A saída de erro.</param>
    /// <returns>Verdadeiro se o comando foi bem sucedido.</returns>
    private static bool RunCmd(string command, bool emitStdOutput, bool emitErrOutput, out string stdOutput, out string errOutput)
    {
      stdOutput = null;
      errOutput = null;

      try
      {
        string stdOutputFile = null;
        string errOutputFile = null;

        var regex = new Regex(" (2?>)([^>]*)");
        var matches = regex.Matches(command);
        foreach (var match in matches.Cast<Match>())
        {
          var kind = match.Groups[1].Value;
          var path = match.Groups[2].Value.Trim().Replace("'", "\"");

          if (kind == "2>")
            errOutputFile = path;
          else
            stdOutputFile = path;

          var text = match.Captures[0].Value;
          command = command.Replace(text, "");
        }

        var info = ParseCommand(command);
        info.UseShellExecute = false;
        info.RedirectStandardOutput = emitStdOutput || (stdOutputFile != null);
        info.RedirectStandardError = emitErrOutput || (errOutputFile != null);

        if (stdOutputFile != null) stdOutputFile.GetDir().MkDir();
        if (errOutputFile != null) errOutputFile.GetDir().MkDir();

        var redirects = "";
        if (emitStdOutput) redirects += $" > nul";
        if (emitErrOutput) redirects += $" 2> nul";
        if (stdOutputFile != null) redirects += $" > {stdOutputFile.Quote()}";
        if (errOutputFile != null) redirects += $" 2> {errOutputFile.Quote()}";

        Prompt.PrintLine();
        Prompt.PrintLine($"[cmd] {info.FileName.Quote()} {info.Arguments} {redirects}");

        using (var process = Process.Start(info))
        {
          if (emitStdOutput)
          {
            stdOutput = process.StandardOutput.ReadToEnd().Trim();
            process.StandardOutput.Close();
          }
          else if (stdOutputFile != null)
          {
            File.WriteAllText(stdOutputFile, process.StandardOutput.ReadToEnd());
            process.StandardOutput.Close();
          }

          if (emitErrOutput)
          {
            errOutput = process.StandardError.ReadToEnd().Trim();
            process.StandardError.Close();
          }
          else if (errOutputFile != null)
          {
            File.WriteAllText(errOutputFile, process.StandardError.ReadToEnd());
            process.StandardError.Close();
          }

          process.WaitForExit();
          
          return process.ExitCode == 0;
        }
      }
      catch (Exception ex)
      {
        Prompt.PrintFault(ex);
        return false;
      }
    }
    
    private static ProcessStartInfo ParseCommand(string command)
    {
      string cmd;
      string arg;

      command = command.Replace("'", "\"");

      if (command.StartsWith("\""))
      {
        var index = command.IndexOf('"', 1);
        cmd = command.Substring(1, index - 1);
        arg = command.Substring(index + 1).Trim();
      }
      else
      {
        cmd = command.Split(' ').First();
        arg = command.Substring(cmd.Length).Trim();
      }

      var info = new ProcessStartInfo(cmd, arg);
      return info;
    }

    #endregion

    #region Comandos da web

    /// <summary>
    /// Realiza o download de uma URL.
    /// Se um nome de arquivo não for indicado o nome é inferido da própria URL e
    /// salvo na pasta corrente.
    /// </summary>
    /// <param name="url">A URL para download.</param>
    /// <param name="filepath">Caminho opcional para gravação do arquivo baixado.</param>
    public static void Download(this string url, string filepath = null)
    {
      Prompt.PrintLine();
      Prompt.PrintLine($"[down] {url}");

      using (var web = new WebClient())
      {
        web.Credentials = new NetworkCredential("subversion", "#qwer0987");
        if (filepath == null)
        {
          filepath = url.Split('/').Last();
          if (string.IsNullOrWhiteSpace(filepath))
          {
            throw new PromptException($"Impossível determinar o nome do arquivo destino: {url}");
          }
        }
        web.DownloadFile(url, filepath);
      }
    }

    #endregion

  }
}
