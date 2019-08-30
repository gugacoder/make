using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PackDm.Model;

namespace PackDm.IO
{
  public class InputCollector
  {
    private Options options;

    public InputCollector(Options options)
    {
      this.options = options;
    }

    public string CollectData(params string[] messageLines)
    {
      if (options.NonInteractiveOn)
        return null;

      foreach (var line in messageLines)
      {
        Console.WriteLine(line);
      }
      Console.Write("> ");

      var data = Console.ReadLine();
      return data;
    }

    public string CollectSecret(params string[] messageLines)
    {
      // A coleta de caracter em segredo, para senhas por exemplo, ainda
      // não está implementada
      return CollectData(messageLines);
    }
  }
}
