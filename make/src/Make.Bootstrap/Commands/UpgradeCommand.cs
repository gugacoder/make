using Make.Bootstrap.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Make.Bootstrap.Settings;

namespace Make.Bootstrap.Commands
{
  class UpgradeCommand : ICommand
  {
    public int Run(Action<string> logger)
    {
      string tempfile = null;
      try
      {
        tempfile = Path.GetTempFileName();
        var destfile = Path.GetFullPath(ToolName);

        logger.Invoke($"Baixando arquivo... {ToolName}");

        RemoteFiles.CopyFile($"dist/{Platform}/{ToolName}", tempfile);
        File.Copy(tempfile, destfile, overwrite: true);

        logger.Invoke($"[OK]Arquivo baixado: {ToolName}");
        return 0;
      }
      finally
      {
        try
        {
          if (File.Exists(tempfile))
          {
            File.Delete(tempfile);
          }
        }
        catch { /* Nada a fazer */ }
      }
    }
  }
}