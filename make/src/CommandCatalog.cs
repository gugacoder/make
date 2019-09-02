using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Make.Library.Posix;

namespace Make
{
  class CommandCatalog : IEnumerable<object>
  {
    private ICommand[] Commands { get; set; }

    public CommandCatalog()
    {
      Commands = (
        from type in this.GetType().Assembly.DefinedTypes
        where !type.IsInterface
           && typeof(ICommand).IsAssignableFrom(type)
        let instance = CreateCommand(type)
        where instance != null
        select instance
      ).ToArray();
    }

    private ICommand CreateCommand(TypeInfo type)
    {
      try
      {
        var command = (ICommand)Activator.CreateInstance(type);
        return command;
      }
      catch (Exception ex)
      {
        Prompt.PrintWarn(ex, $"Não foi possível instanciar o comando: {type.FullName}");
        return null;
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
