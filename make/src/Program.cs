using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Diagnostics;
using Make.Library.Posix;
using System.Security.Cryptography;
using Make.Helpers;

namespace Make
{
  static class Program
  {
    static int Main(string[] args)
    {
      try
      {
        Toolchain.Initialize();

        ServicePointManager.ServerCertificateValidationCallback
          = (o, cert, chain, err) => true;

        var options = PosixParser.ParseArgs<ProgramOptions>(args: args, lenient: true);

        if (options.Help.On)
        {
          PosixPrinter.PrintHelp(options);
          return 0;
        }

        if (options.Command.On)
        {
          var commandName = options.Command.Text;
          var commandArgs = args.Skip(1).ToArray();
          return CallCommand(commandName, commandArgs);
        }

        Prompt.PrintInvalidUsage("Nenhum comando foi indicado na linha de comando.");
        return 1;
      }
      catch (Exception ex) when (ex is InvalidUsageException || ex?.InnerException is InvalidUsageException)
      {
        ex = (ex as InvalidUsageException) ?? (ex.InnerException as InvalidUsageException);
        Prompt.PrintInvalidUsage(ex.Message);
        return 1;
      }
      catch (Exception ex) when (ex is PromptException || ex?.InnerException is PromptException)
      {
        ex = (ex as PromptException) ?? (ex.InnerException as PromptException);
        Prompt.PrintFault(ex.Message);
        return 1;
      }
      catch (Exception ex)
      {
        Prompt.PrintFault(ex, null);
        return 1;
      }
    }

    private static int CallCommand(string commandName, string[] commandArgs)
    {
      var commands = new CommandCatalog();
      var command = (
        from x in commands
        let name = CommandCatalog.GetCommandName(x)
        where name == commandName
        select x
      ).FirstOrDefault();

      if (command == null)
      {
        Prompt.PrintInvalidUsage($"Comando não encontrado: {commandName}");
        return 1;
      }

      var commandMethod = CommandCatalog.GetCommandMethod(command);
      
      var commmandOptionsType = CommandCatalog.GetOptionsType(command);
      var commmandOptions = Activator.CreateInstance(commmandOptionsType);
      
      PosixParser.ParseArgs(commmandOptions, commandArgs);

      var result = commandMethod.Invoke(command, new[] { commmandOptions });

      if (result is int)
        return (int)result;

      if (result is bool)
        return true.Equals(result) ? 0 : 1;

      return 0;
    }
  }
}
