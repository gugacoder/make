using Make.Bootstrap.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Bootstrap
{
  static class Program
  {
    private static bool Verbose;

    public static int Main(string[] args)
    {
      try
      {
        var command = "Upgrade";

        var enumerator = args.Cast<string>().GetEnumerator();
        while (enumerator.MoveNext())
        {
          var option = enumerator.Current;
          switch (option)
          {
            case "-v":
            case "--verbose":
              Verbose = true;
              break;

            case "-h":
            case "--help":
            case "/?":
              command = "Help";
              break;

            default:
              return PrintInvalidUsage(option);
          }
        }

        var @namespace = typeof(ICommand).Namespace;
        var typeName = $"{@namespace}.{command}Command";
        var type = Type.GetType(typeName);
        var instance = (ICommand)Activator.CreateInstance(type);

        var status = instance.Run(Console.WriteLine);

        return status;
      }
      catch (Exception ex)
      {
        DumpException(ex);
        return 9;
      }
    }

    private static int PrintInvalidUsage(string option = null)
    {
      Console.WriteLine($"Uso incorreto!");
      if (option != null) Console.WriteLine($"  Parâmetro não suportado: {option}");
      Console.WriteLine($"Para mais detalhes use:");
      Console.WriteLine($"  {Settings.BootstrapName} --help");
      Console.WriteLine($"Copyright (c) KeepCoding 2019");
      return 1;
    }

    public static void DumpException(Exception ex)
    {
      Console.Write("[ERROR]");
      Console.WriteLine(ex.Message);
      if (Verbose)
      {
        Console.WriteLine(ex.GetStackTrace());
      }
    }
  }
}