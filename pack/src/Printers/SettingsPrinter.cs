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
  public class SettingsPrinter
  {
    public void PrintSettings(Settings settings)
    {
      PrintSpacing();
      PrintComment("");
      PrintComment("Configurações para execução de cliente:");
      PrintComment("");
      if (settings.RepositoryUris.Any())
      {
        PrintPropertyName(Conf.SourceUriKey);
        foreach (var uri in settings.RepositoryUris)
        {
          PrintPropertyValue(uri);
        }
      }

      PrintSpacing();
      PrintComment("");
      PrintComment("Configurações para execução de serviço:");
      PrintComment("");
      PrintProperty(Conf.FolderKey, settings.RepositoryFolder);
      PrintProperty(Conf.PortKey, settings.Port);

      PrintSpacing();
      PrintComment("");
      PrintComment("Configurações gerais:");
      PrintComment("");
      PrintProperty(Conf.ProxyKey, settings.Proxy);
      PrintProperty(Conf.ProxyHttpsKey, settings.ProxyHttps);
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

