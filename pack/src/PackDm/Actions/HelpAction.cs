using System;
using PackDm.Model;
using System.Net;
using System.IO;
using System.Collections.Generic;
using PackDm.Algorithms;
using PackDm.Printers;

namespace PackDm.Actions
{
  public class HelpAction : IAction
  {
    public static bool IsHelpAction(Options options)
    {
      return options.VersionOn
          || options.HelpOn
          || options.HelpSettingsOn
          || options.HelpPackOn
          || options.HelpEffectivePackOn;
    }

    public void Proceed(Context context)
    {
      var options = context.Options;

      if (options.VersionOn)
        PrintVersion(context);
      
      if (options.HelpOn)
        PrintHelp(context);
      
      if (options.HelpSettingsOn)
        PrintSettings(context);

      if (options.HelpPackOn)
        PrintPack(context);

      if (options.HelpEffectivePackOn)
        PrintEffectivePack(context);
    }

    private void PrintVersion(Context context)
    {
      Console.WriteLine(Program.Version);
    }

    private void PrintHelp(Context context)
    {
      var printer = new OptionsPrinter();
      printer.PrintOptions();
    }

    private void PrintSettings(Context context)
    {
      var printer = new SettingsPrinter();
      printer.PrintSettings(context.Settings);
    }

    private void PrintPack(Context context)
    {
      var printer = new PackPrinter();
      printer.PrintPack(context.Pack);
    }

    private void PrintEffectivePack(Context context)
    {
      var options = context.Options;
      var settings = context.Settings;
      var fileSystem = context.FileSystem;

      var builder = new PackBuilder();
      builder.ClonePropertiesFrom(context.Pack);

      Queue<Exception> exceptions = new Queue<Exception>();
      var success = false;

      foreach (var uri in settings.RepositoryUris)
      {
        try
        {
          builder.ResolveDependencies(uri);
          success = true;
          break;
        }
        catch (Exception ex)
        {
          exceptions.Enqueue(ex);
        }
      }

      if (!success)
      {
        Console.WriteLine("[resolver]Não foi possível resolver dependências.");
        while (exceptions.Count > 0)
        {
          var ex = exceptions.Dequeue();
          if (Program.Verbose)
            Program.DumpException(ex);
          else
            Console.WriteLine("[resolver][err]" + ex.Message);
        }
      }

      builder.ResolveDistribution(fileSystem);
      if (options.SetVersionOn)
      {
        builder.SetVersion(options.SetVersionValue);
      }

      var effectivePack = builder.Pack;

      var printer = new PackPrinter();
      printer.PrintPack(effectivePack);
    }
  }
}