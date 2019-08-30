using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using Do.Library.Posix;
using Do.Library.Subversion;
using Do.Library.Shell;
using Do.Library.PackDm;
using System.Text.RegularExpressions;
using Do.PlugIns.Msi;
using Do.Library.Projects;

namespace Do.PlugIns.VisualStudio
{
  [Command]
  class PrintVersionCommand
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
