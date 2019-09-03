using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackDm.Bootstrap.Commands
{
  class HelpCommand : ICommand
  {
    public int Run(Action<string> logger)
    {
      Console.WriteLine($"Atualizador de versão do PackDm.");
      Console.WriteLine($"O atualizador atualiza a ferramenta principal `{Settings.ToolName}'.");
      Console.WriteLine($"Uso:");
      Console.WriteLine($"  {Settings.BootstrapName} [ -v ]");
      Console.WriteLine($"Parâmetros:");
      Console.WriteLine($"  -v, --verbose");
      Console.WriteLine($"          Imprime falhas detalhadas e mensagens adicionais.");
      Console.WriteLine($"Copyright (c) KeepCoding 2019");
      return 0;
    }
  }
}