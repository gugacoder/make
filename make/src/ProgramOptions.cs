using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Posix;

namespace Make
{
  [HelpTitle("Ferramenta de compilação de projetos.")]
  [HelpUsage("comando [ opcoes ... ]")]
  class ProgramOptions
  {
    public Opt Help { get; set; }
    
    [Operand]
    public OptArg Command { get; set; }

    [HelpSection("Comandos", Order = SectionOrder.AfterUsage)]
    public string PrintCommands()
    {
      var text = CreateAvailableCommandsHelpText();
      return text;
    }

    public static string CreateAvailableCommandsHelpText()
    {
      var catalog = new CommandCatalog();
      var lines =
        from command in catalog
        let optionsType = CommandCatalog.GetOptionsType(command)
        let options = Activator.CreateInstance(optionsType)
        let name = CommandCatalog.GetCommandName(command)
        let title = Prompt.GetAppTitle(options)
        orderby name
        select new[] {
          name,
          ' '.Replicate(12) + title
        };
      var text = lines.SelectMany().JoinLines();
      return text;
    }
  }
}
