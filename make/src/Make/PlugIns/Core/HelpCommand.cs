using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Posix;

namespace Make.PlugIns.Core
{
  class HelpCommand : ICommand<HelpCommandOptions>
  {
    public bool Exec(HelpCommandOptions options)
    {
      if (options.Command.On)
      {
        var catalog = new CommandCatalog();

        var commandName = options.Command.Text;
        var command =
          catalog
            .Where(x => CommandCatalog.GetCommandName(x) == commandName)
            .FirstOrDefault();

        if (command == null)
        {
          Prompt.PrintInvalidUsage($"Comando não encontrado: {commandName}");
          return false;
        }

        var commandOptionsType = CommandCatalog.GetOptionsType(command);
        var commandOptions = Activator.CreateInstance(commandOptionsType);
        PosixPrinter.PrintHelp(commandOptions);
      }
      else
      {
        PosixPrinter.PrintHelp(options);
      }

      return true;
    }
  }
}
