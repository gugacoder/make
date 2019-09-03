using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Make.Library.Posix;
using Make.Library.Subversion;
using Make.Library.Shell;
using Make.Library.PackDm;
using System.Text.RegularExpressions;
using Make.Library.Projects;

namespace Make.PlugIns.VisualStudio
{
  class PrintVersionCommand : ICommand<PrintVersionCommandOptions>
  {
    public bool Exec(PrintVersionCommandOptions options)
    {
      var loader = new ProjectLoader();
      loader.PackConf = options.ConfFile.Text;
      loader.PackInfo = options.PackFile.Text;
      var ret = loader.LoadProjectHeader();
      if (!ret.Ok) return Prompt.PrintCannotContinue();

      var project = ret.Value;
      var version = project.Version;

      // imprimindo no console e no arquivo
      Prompt.PrintInfo(version);

      return true;
    }
  }
}
