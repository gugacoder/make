﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Posix;

namespace Make.PlugIns.Core
{
  [HelpTitle("Comando de impressão de ajuda de comandos.")]
  [HelpUsage("help comando")]
  class HelpCommandOptions
  {
    [Operand]
    public OptArg Command { get; set; }

    [HelpSection("Comandos", Order = SectionOrder.AfterUsage)]
    public string PrintCommands()
    {
      var text = ProgramOptions.CreateAvailableCommandsHelpText();
      return text;
    }
  }
}
