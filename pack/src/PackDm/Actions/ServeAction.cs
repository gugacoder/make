using System;
using PackDm.Model;
using System.Net;
using System.IO;
using System.Collections.Generic;
using PackDm.Service;

namespace PackDm.Actions
{
  public class ServeAction : IAction
  {
    public void Proceed(Context context)
    {
      LaunchService(context);
    }

    private void LaunchService(Context context)
    {
      var settings = context.Settings;
      var options = context.Options;
     
      var host = PackHostFactory.CreateFileHost(settings, options);
      host.Open();

      Console.WriteLine("Serviço iniciado em: " + host.Uri);
      Console.WriteLine("Pressione Ctrl+C para encerrar...");

      // evita a emissão do sinal de encerramento do processo,
      // mantendo o processo em execução até Program.TerminalSignal.Set()
      // ser invocado ou até o processo ser derrubado.
      Program.TerminalSignal.Reset();
    }
  }
}

