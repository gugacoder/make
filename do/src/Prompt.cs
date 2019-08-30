using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;
using Do.Library.Posix;

namespace Do
{
  /// <summary>
  /// Utilitários de linha de comando.
  /// </summary>
  public class Prompt
  {
    public static string GetAppName(object target)
    {
      if (target is Type)
        throw new InvalidUsageException($"Era esperado um objeto mas foi recebido um tipo: {((Type)target).FullName}");

      return Assembly.GetEntryAssembly().GetName().Name;
    }

    public static string GetAppTitle(object target)
    {
      if (target is Type)
        throw new InvalidUsageException($"Era esperado um objeto mas foi recebido um tipo: {((Type)target).FullName}");

      var type = (target is Type) ? (Type)target : target.GetType();
      var appTitle = (
         from a in type.GetCustomAttributes(true).OfType<HelpTitleAttribute>()
         select a.Text
      ).SingleOrDefault();
      return appTitle;
    }

    /// <summary>
    /// Imprime uma mensagem informativa.
    /// </summary>
    /// <returns>Um status de saída indicando sucesso.</returns>
    public static bool PrintInfo(string message)
    {
      WriteErrorLine("");
      WriteError("[info]");
      var lines = message.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x));
      foreach (var line in lines)
      {
        WriteErrorLine(line);
      }
      return true;
    }

    /// <summary>
    /// Imprime uma mensagem informativa.
    /// </summary>
    /// <returns>Um status de saída indicando sucesso.</returns>
    public static bool PrintInfo(string format, params object[] args)
    {
      WriteErrorLine("");
      WriteError("[info]");
      WriteErrorLine(format, args);
      return true;
    }

    /// <summary>
    /// Imprime uma nota de uso incorreto.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintInvalidUsage(string cause)
    {
      var appName = Assembly.GetEntryAssembly().GetName().Name;

      WriteErrorLine("Uso incorreto.");
      if (cause != null)
      {
        var lines = cause.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x));
        foreach (var line in lines)
        {
          WriteError("  ");
          WriteErrorLine(line);
        }
      }
      WriteErrorLine("Para mais detalhes use:");
      WriteError("  ");
      WriteError(appName);
      WriteErrorLine(" --help");

      return false;
    }

    /// <summary>
    /// Imprime uma nota de uso incorreto.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintInvalidUsage(string format, params object[] args)
    {
      PrintInvalidUsage(string.Format(format, args));

      return false;
    }

    /// <summary>
    /// Imprime uma mensagem de alerta.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintWarn(string message)
    {
      WriteErrorLine("");
      WriteError("[warn]");
      var lines = message.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x));
      foreach (var line in lines)
      {
        WriteErrorLine(line);
      }
      return false;
    }

    /// <summary>
    /// Imprime uma mensagem de alerta.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintWarn(string format, params object[] args)
    {
      WriteErrorLine("");
      WriteError("[warn]");
      WriteErrorLine(format, args);
      return false;
    }

    /// <summary>
    /// Imprime uma mensagem de erro.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintFault(Exception ex)
    {
      PrintFault(ex, null);
      return false;
    }

    /// <summary>
    /// Imprime uma mensagem de erro.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintFault(string message)
    {
      PrintFault(null, message);
      return false;
    }

    /// <summary>
    /// Imprime uma mensagem de erro.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintFault(Exception ex, string message)
    {
      WriteErrorLine("");

      if (ex is TargetInvocationException)
        ex = ex.InnerException;

      if (message != null)
      {
        WriteError("[error]");
        var lines = message.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x));
        foreach (var line in lines)
        {
          WriteErrorLine(line);
        }
      }

      while (ex != null)
      {
        if (!(ex is TargetInvocationException))
        {
          WriteError("[fault]");
          WriteErrorLine(ex.Message);
          WriteError("cause ");
          WriteErrorLine(ex.GetType().Name);
          if (ex.StackTrace != null)
          {
            WriteErrorLine(ex.StackTrace);
          }
        }
        ex = ex.InnerException;
      }
      return false;
    }

    /// <summary>
    /// Imprime uma mensagem de erro.
    /// </summary>
    public static bool PrintFault(Exception ex, string format, params object[] args)
    {
      PrintFault(ex, string.Format(format, args));
      return false;
    }

    /// <summary>
    /// Imprime um alerta quando não é possível prosseguir com o comando.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintCannotContinue()
    {
      PrintFault("-- NÃO SERÁ POSSÍVEL CONTINUAR --");
      return false;
    }

    /// <summary>
    /// Imprime um alerta quando não é possível prosseguir com o comando.
    /// </summary>
    /// <param name="message">Uma mensagem explicativa.</param>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintCannotContinue(string message)
    {
      PrintWarn(message);
      PrintFault("-- NÃO SERÁ POSSÍVEL CONTINUAR --");
      return false;
    }

    /// <summary>
    /// Exibe um alerta quando não há nada para ser feito.
    /// </summary>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintNothingToBeDone()
    {
      PrintFault("Não há nada para ser feito.");
      return false;
    }

    /// <summary>
    /// Exibe um alerta quando não há nada para ser feito.
    /// </summary>
    /// <param name="message">Uma mensagem explicativa.</param>
    /// <returns>Um status de saída indicando falha.</returns>
    public static bool PrintNothingToBeDone(string message)
    {
      PrintWarn(message);
      PrintFault("Não há nada para ser feito.");
      return false;
    }

    /// <summary>
    /// Imprime uma mensagem diretamente.
    /// </summary>
    public static void PrintLine()
    {
      WriteErrorLine("");
    }

    /// <summary>
    /// Imprime uma mensagem diretamente.
    /// </summary>
    public static void PrintLine(string message)
    {
      var lines = message.Split('\n', '\r').Where(x => !string.IsNullOrWhiteSpace(x));
      foreach (var line in lines)
      {
        WriteErrorLine(line);
      }
    }

    /// <summary>
    /// Imprime uma mensagem diretamente.
    /// </summary>
    public static void PrintLine(string format, params object[] args)
    {
      WriteErrorLine(format, args);
    }

    public static void WriteInfo(string text, params object[] args)
    {
      if (text != null && args.Length > 0)
        text = string.Format(text, args);

      text = SafeText(text);

      Console.Out.Write(text);
      Debug.Write(text);
    }

    public static void WriteInfoLine(string text, params object[] args)
    {
      if (text != null && args.Length > 0)
        text = string.Format(text, args);

      text = SafeText(text);

      Console.Out.WriteLine(text);
      Debug.WriteLine(text);
    }

    public static void WriteError(string text, params object[] args)
    {
      if (text != null && args.Length > 0)
        text = string.Format(text, args);

      text = SafeText(text);

      Console.Error.Write(text);
      Debug.Write(text);
    }

    public static void WriteErrorLine(string text, params object[] args)
    {
      if (text != null && args.Length > 0)
        text = string.Format(text, args);

      text = SafeText(text);

      Console.Error.WriteLine(text);
      Debug.WriteLine(text);
    }

    public static string SafeText(string text)
    {
      if (text == null) return text;
      return text.RegexReplace("--password [^ ]+", "--password ***");
    }
  }
}
