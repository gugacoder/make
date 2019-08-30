using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using PackDm.Algorithms;
using PackDm.Model;

namespace PackDm.Printers
{
  public class PackPrinter
  {
    public void PrintPack(Pack pack)
    {
      PrintComment("");
      PrintComment("Identificação do artefato:");
      PrintComment("");
      PrintProperty("pack", pack.Id);

      if (pack.Deps.Any())
      {
        PrintSpacing();
        PrintComment("");
        PrintComment("Dependências do artefato:");
        PrintComment("");
        PrintPropertyName("deps");
        foreach (var artifact in pack.Deps)
        {
          PrintPropertyValue(artifact.Id);
        }
      }

      if (pack.Dist.Any())
      {
        PrintSpacing();
        PrintComment("");
        PrintComment("Arquivos componentes do artefato:");
        PrintComment("");
        PrintPropertyName("dist");
        foreach (var file in pack.Dist)
        {
          PrintPropertyValue(file);
        }
      }
    }

    #region Métodos internos de impressão no formado de arquivo de configuração...

    private void PrintProperty(string key, object value)
    {
      Console.WriteLine(key);
      Console.Write("  ");
      Console.WriteLine(value ?? "(null)");
    }

    private void PrintPropertyName(string key)
    {
      Console.WriteLine(key);
    }

    private void PrintPropertyValue(object value)
    {
      Console.Write("  ");
      Console.WriteLine(value ?? "(null)");
    }

    private void PrintComment(string comment)
    {
      Console.Write("# ");
      Console.WriteLine(comment);
    }

    private void PrintSpacing()
    {
      Console.WriteLine();
    }

    #endregion

  }
}

