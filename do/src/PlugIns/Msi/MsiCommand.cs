using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.PlugIns.Msi
{
  [Command]
  class MsiCommand
  {
    public bool Exec(MsiCommandOptions options)
    {
      //
      // Validando parametros
      //
      if (!options.Operands.On)
        return Prompt.PrintNothingToBeDone("Nenhum comando foi indicado para o módulo MSI");

      //
      // Executando o comando
      //
      var command = options.Operands.Items.First();
      var actionName = command.ChangeCase(TextCase.PascalCase);
      var actionTypeName = $"{GetType().Namespace}.{actionName}Action";
      var actionType = Type.GetType(actionTypeName);

      if (actionType == null)
        return Prompt.PrintInvalidUsage("Comando não suportado: " + command);

      var action = Activator.CreateInstance(actionType) as IAction;

      if (action == null)
        return Prompt.PrintInvalidUsage("Comando não suportado: " + command);

      return action.Exec(options);
    }
  }
}