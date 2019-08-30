using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;

namespace PackDm.Bootstrap
{
  public class Options
  {
    public bool VersionOn { get; set; }

    public bool HelpOn { get; set; }
    
    public bool SelfOn { get; set; }
    
    public bool CustomUriOn { get; set; }    
    public string CustomUriValue { get; set; }
    
    public bool TimeoutOn { get; set; }
    public string TimeoutValue { get; set; }

    public bool VerboseOn { get; set; }

    public static Options Parse(string[] args)
    {
      var options = new Options();

      var queue = new Queue<string>(args);
      while (queue.Count > 0)
      {
        var arg = queue.Dequeue();
        switch (arg)
        {
          case "--version":
            {
              options.VersionOn = true;
              break;
            }

          case "-h":
          case "--help":
            {
              options.HelpOn = true;
              break;
            }

          case "--self":
            {
              options.SelfOn = true;
              break;
            }

          case "--uri":
            {
              var value = queue.Dequeue();
              options.CustomUriOn = true;
              options.CustomUriValue = value;
              break;
            }

          case "--timeout":
            {
              var value = queue.Dequeue();
              options.TimeoutOn = true;
              options.TimeoutValue = value;
              break;
            }

          case "--verbose":
            {
              options.VerboseOn = true;
              break;
            }
        }
      }

      return options;
    }

    public static void PrintHelp()
    {
      var cmd = Assembly.GetEntryAssembly().GetName().Name;

      Console.Write("PackDm.Bootstrap (");
      Console.Write(Program.Version);
      Console.WriteLine(")");
      Console.WriteLine("Atualizador do gerenciador de dependências PackDm.");
      Console.WriteLine("Uso:");
      Console.WriteLine("  " + cmd + " [ --uri URI ]");
      Console.WriteLine("Opções:");

      Console.WriteLine("  --version");
      Console.WriteLine("      Imprime a versão do aplicativo.");

      Console.WriteLine("  --help, -h");
      Console.WriteLine("      Imprime esta ajuda.");

      Console.WriteLine("  --uri URI");
      Console.WriteLine("      URI personalizada para download dos aplicativos do PackDm.");
      Console.WriteLine("      Por padrão os endereços procurados são:");
      foreach (var uri in Updater.DefaultUris)
      {
        Console.Write("        ");
        Console.WriteLine(uri);
      }

      Console.WriteLine("  --self");
      Console.WriteLine("      Força a atualização do próprio `" + cmd + "`.");

      Console.WriteLine("  --timeout SEGUNDOS");
      Console.WriteLine("      Modifica o tempo de espera para conexões HTTP.");
      Console.WriteLine("      O tempo de espera padrão é de " + Updater.DefaultTimeout + " segundos.");

      Console.WriteLine("  --verbose");
      Console.WriteLine("      Imprime mensagens detalhadas.");

      Console.WriteLine("Copyright © KeepCoding 2019");
    }

  }
}
