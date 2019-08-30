using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using PackDm.Algorithms;
using PackDm.Model;
using PackDm.Handlers;

namespace PackDm.Printers
{
  public class OptionsPrinter
  {
    public void PrintOptions()
    {
      var cmd = Assembly.GetEntryAssembly().GetName().Name;

      var info = OptionsHandler.ExtractInfo();
      var actions = info.SingleOrDefault(x => x.Definition.IsNameless);
      var arguments = info.Where(x => !x.Definition.IsNameless);

      PrintLine("PackDm (", Program.Version, ")");
      PrintLine("Gerenciamento simplificado de dependências.");
      PrintLine("Por padrão os artefatos são baixados e publicados na URL:");
      PrintLine("  ", Settings.DefaultSourceUri);
      PrintLine("a menos que configurado de outra forma no arquivo `pack.conf`.");

      PrintLine("Uso:");
      PrintLine("  ", cmd, " AÇÕES [ OPÇÕES ]");
      if (actions != null)
      {
        foreach (var line in actions.Description.TextLines)
        {
          PrintLine(line);
        }
      }

      var categories = 
        from a in arguments
        group a by a.Definition.Category into category
        select new {
          Name = category.Key,
          Arguments = category
        };
      foreach (var category in categories)
      {

        var name = string.IsNullOrWhiteSpace(category.Name) ? "Opções" : category.Name;
        PrintLine(name, ":");

        foreach (var argument in category.Arguments)
        {
          Print("  --");
          Print(argument.Definition.LongName);
          if (argument.Definition.ShortName != default(char))
          {
            Print(", -");
            Print(argument.Definition.ShortName);
          }
          if (argument.HasValue)
          {
            Print(" VALOR");
          }
          PrintLine();

          foreach (var line in argument.Description.TextLines)
          {
            PrintLine("      ", line);
          }
        }
      }

      PrintCopyright();
    }

    #region Métodos internos de impressão simples...

    private void Print(object text, params object[] others)
    {
      Console.Write(text);
      foreach (var other in others)
      {
        Console.Write(other);
      }
    }

    private void PrintLine()
    {
      Console.WriteLine();
    }

    private void PrintLine(object text, params object[] others)
    {
      Print(text, others);
      Console.WriteLine();
    }

    private void PrintCopyright()
    {
      Console.WriteLine("Copyright © KeepCoding 2019");
    }

    #endregion

  }
}

