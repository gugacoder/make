using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;

namespace PackDm.Bootstrap
{
  class Program
  {
    public static Version Version
    {
      get { return Assembly.GetExecutingAssembly().GetName().Version; }
    }

    static void Main(string[] args)
    {
      try
      {
        var options = Options.Parse(args);
        if (options.VersionOn)
        {
          Console.WriteLine(Version);
        }
        else if (options.HelpOn)
        {
          Options.PrintHelp();
        }
        else
        {
          var updater = new Updater();
          updater.UpdatePackDm(options);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Falhou a tentativa de atualizar o sistema.");
        DumpException(ex, true);
      }
    }

    public static void DumpException(Exception ex, bool verbose)
    {
      if (verbose)
      {
        while (ex != null)
        {

          Console.Write("Causa: ");
          Console.Write(ex.GetType());
          Console.Write(": ");
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.StackTrace);
          ex = ex.InnerException;
        }
      }
      else
      {
        Console.WriteLine(ex.Message);
      }
    }

  }
}
