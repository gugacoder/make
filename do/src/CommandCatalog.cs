using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Do.Library.Posix;

namespace Do
{
  class CommandCatalog : IEnumerable<object>
  {
    [ImportMany(CommandAttribute.ContractName)]
    private object[] Commands { get; set; }

    public CommandCatalog()
    {
      using (var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
      using (var composition = new CompositionContainer(catalog))
      {
        composition.ComposeParts(this);
      }
    }

    public IEnumerator<object> GetEnumerator()
    {
      return Commands.Cast<object>().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return Commands.GetEnumerator();
    }

    #region Utilitários estáticos

    public static MethodInfo GetCommandMethod(object command)
    {
      var commandMethod = command.GetType().GetMethod("Exec");
      if (commandMethod == null || commandMethod.GetParameters().Count() != 1)
        throw new InvalidUsageException($"Comando não implementado corretamente: Comando \"Exec()\" ausente ou incorreto, no tipo: {command.GetType().FullName}");
      return commandMethod;
    }

    public static Type GetOptionsType(object command)
    {
      var commandMethod = GetCommandMethod(command);
      var commmandOptionsType = commandMethod.GetParameters().First().ParameterType;
      return commmandOptionsType;
    }

    public static string GetCommandName(object command)
    {
      var typeName = command.GetType().Name;
      var commandName = typeName.Replace("Command", "").ChangeCase(TextCase.Hyphenated);
      return commandName;
    }

    #endregion
  }
}
