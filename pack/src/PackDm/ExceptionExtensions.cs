using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PackDm
{
  /// <summary>
  /// Coleção de extensões para ampliar o feedback produzido para exceções
  /// lançadas pelo sistema.
  /// </summary>
  public static class ExceptionExtensions
  {
    /// <summary>
    /// Obtém uma descrição completa da pilha de erros da exceção indicada e
    /// suas causas.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    /// <returns>A descrição completa da pilha de erros.</returns>
    public static string GetStackTrace(this Exception exception)
    {
      using (var writer = new StringWriter())
      {
        Trace(exception, writer);
        return writer.ToString();
      }
    }

    /// <summary>
    /// Emite uma cópia da pilha de erros completa no System.Diagnostics.Debug.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    public static void Debug(this Exception exception)
    {
      try
      {
        var stack = GetStackTrace(exception);
        System.Diagnostics.Debug.WriteLine(stack);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
    }

    /// <summary>
    /// Emite uma cópia da pilha de erros completa no canal de diagnóstico
    /// System.Diagnostics.Trace com gravidade tipo TraceError.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    public static void Trace(this Exception exception)
    {
      try
      {
        var stack = GetStackTrace(exception);
        System.Diagnostics.Trace.TraceError(stack);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
    }

    /// <summary>
    /// Emite uma cópia da pilha de erros completa no canal de diagnóstico
    /// System.Diagnostics.Trace com gravidade tipo TraceWarning.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    public static void TraceWarning(this Exception exception)
    {
      var stack = GetStackTrace(exception);
      try
      {
        System.Diagnostics.Trace.TraceWarning(stack);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
    }

    /// <summary>
    /// Emite uma cópia da pilha de erros completa no canal de diagnóstico
    /// System.Diagnostics.Trace com gravidade tipo TraceError.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    /// <param name="message">Mensagem explicativa adicional.</param>
    public static void Trace(this Exception exception, string message)
    {
      var stack = GetStackTrace(exception);
      try
      {
        System.Diagnostics.Trace.TraceError("{0}\ncause:\n{1}", message, stack);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
    }

    /// <summary>
    /// Emite uma cópia da pilha de erros completa no canal de diagnóstico
    /// System.Diagnostics.Trace com gravidade tipo TraceWarning.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    /// <param name="message">Mensagem explicativa adicional.</param>
    public static void TraceWarning(this Exception exception, string message)
    {
      try
      {
        var stack = GetStackTrace(exception);
        System.Diagnostics.Trace.TraceWarning("{0}\ncause:\n{1}", message, stack);
      }
      catch (Exception ex)
      {
        Dump(exception, ex);
      }
    }

    /// <summary>
    /// Emite uma cópia da pilha de erros completa para o stream de saída
    /// indicado.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    /// <param name="output">
    /// O stream para escrita da pilha de erros completa
    /// </param>
    public static void Trace(this Exception exception, Stream output)
    {
      using (var writer = new StreamWriter(output))
      {
        Trace(exception, writer);
      }
    }

    /// <summary>
    /// Emite uma cópia da pilha de erros completa para o stream de saída
    /// indicado.
    /// </summary>
    /// <param name="exception">A exceção ocorrida.</param>
    /// <param name="output">
    /// O stream para escrita da pilha de erros completa
    /// </param>
    public static void Trace(this Exception exception, TextWriter output)
    {
      output.Write("fault ");
      Exception ex = exception;
      do
      {
        output.WriteLine(ex.Message);
        output.Write(" type ");
        output.WriteLine(ex.GetType().FullName);

        var trace = ex.StackTrace;
        if (trace != null)
        {
          output.Write(trace);
          if (!trace.EndsWith(Environment.NewLine))
          {
            output.WriteLine();
          }
        }

        ex = ex.InnerException;
        if (ex != null)
        {
          output.Write("cause ");
        }
      } while (ex != null);
    }

    /// <summary>
    /// Este método tenta registrar as exceções da forma possível.
    /// Deve ser usado em caso de falha do método geral de gravação de
    /// exceções.
    /// </summary>
    /// <param name="excecoes">As exceções a serem gravadas.</param>
    private static void Dump(params Exception[] excecoes)
    {
      foreach (var exception in excecoes)
      {
        try
        {
          var stack = GetStackTrace(exception);

          Console.Write("[FALHA]");
          Console.WriteLine(exception.Message);
          Console.WriteLine(stack);

          System.Diagnostics.Debug.Write("[FALHA]");
          System.Diagnostics.Debug.WriteLine(exception.Message);
          System.Diagnostics.Debug.WriteLine(stack);
        }
        catch
        {
          // nada a fazer
        }
      }
    }
  }
}