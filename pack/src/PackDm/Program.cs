using System;
using System.Linq;
using System.Configuration;
using System.IO;
using System.Collections.Generic;
using PackDm.Model;
using PackDm.Actions;
using System.Threading;
using PackDm.IO;
using PackDm.Algorithms;
using PackDm.Handlers;
using System.Reflection;
using PackDm.SchemaModel;
using System.Diagnostics;

namespace PackDm
{
  static class Program
  {
    public static readonly EventWaitHandle TerminalSignal = new AutoResetEvent(true);

    public static bool Verbose
    {
      get;
      set;
    }

    public static Version Version
    {
      get { return Assembly.GetExecutingAssembly().GetName().Version; }
    }

    public static int Main(string[] args)
    {
      try
      {
        var status = Proceed(args);
        return status;
      }
      catch (Exception ex)
      {
        DumpException(ex);
        return ex.GetHashCode();
      }
    }

    public static int Proceed(string[] args)
    {
      var options = ParseOptions(args);
      var context = CreateContext(options);

      Program.Verbose = options.VerboseOn;
      ApplySystemSettings(context);

      var actions = EnumerateActions(context).ToArray();
      if (actions.Length == 0)
      {
        var cmd = Assembly.GetEntryAssembly().GetName().Name;
        Console.WriteLine("Nada a fazer. É necessário informar pelo menos uma ação.");
        Console.WriteLine("Para mais detalhes use:");
        Console.Write("  ");
        Console.Write(cmd);
        Console.WriteLine(" --help");
        return 9;
      }

      foreach (var action in actions)
      {
        action.Proceed(context);
      }
      TerminalSignal.WaitOne();

      return 0;
    }

    private static Options ParseOptions(string[] args)
    {
      return OptionsHandler.Parse(args);
    }

    private static Context CreateContext(Options options)
    {
      return ContextFactory.Create(options);
    }

    private static void ApplySystemSettings(Context context)
    {
      var options = context.Options;
      var settings = context.Settings;
      var credentials = context.Credentials;

      var webClientFactory = WebClientFactory.Current;
      webClientFactory.Credentials = credentials;
      webClientFactory.Proxy = settings.Proxy;
      webClientFactory.ProxyHttps = settings.ProxyHttps;
    }

    private static IEnumerable<IAction> EnumerateActions(Context context)
    {
      var pack = context.Pack;
      var options = context.Options;
      var settings = context.Settings;

      if (HelpAction.IsHelpAction(options))
      {
        yield return new HelpAction();
      }
      else if (options.ChainOn)
      {
        var actions = options.ChainValue.Split(' ');
        var iterator = actions.Cast<string>().GetEnumerator();
        while (iterator.MoveNext())
        {
          var action = iterator.Current;
          switch (action)
          {
            case "init":
              yield return  new InitAction();
              break;

            case "install":
              yield return new InstallAction();
              break;

            case "upgrade":
              yield return new UpgradeAction();
              break;

            case "pack":
              yield return new PackAction();
              break;

            case "deploy":
              yield return new DeployAction();
              break;

            case "serve":
              yield return new ServeAction();
              break;

            case "index":
              yield return new IndexAction();
              break;

            case "authorize":
              yield return new AuthorizeAction();
              break;

            case "search":
              while (iterator.MoveNext())
              {
                options.FilterValue.Add(iterator.Current);
              }
              options.FilterOn = options.FilterValue.Any();
              yield return new SearchAction();
              break;

            default:
              throw new PackDmException("Ação não implementada: " + action);
          }
        }

      }
    }

    public static void DumpException(Exception ex)
    {
      DumpException(ex.Message, ex);
    }

    public static void DumpException(string message, Exception ex)
    {
      Console.Write("[err]");
      Console.WriteLine(ex.Message);
      if (Verbose)
      {
        Console.WriteLine(ex.GetStackTrace());
      }
    }

  }
}
