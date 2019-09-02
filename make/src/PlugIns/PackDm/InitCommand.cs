using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Subversion;
using Make.Library.PackDm;

namespace Make.PlugIns.PackDm
{
  class InitCommand : ICommand<InitCommandOptions>
  {
    public bool Exec(InitCommandOptions options)
    {
      bool ok;

      Toolchain.RequirePackDm(true);
      Toolchain.RequireSubversion(true);

      var pack = new Library.PackDm.PackDm();
      pack.PackConf = options.ConfFile.Text;
      pack.PackInfo = options.PackFile.Text;

      var svn = new Svn();

      ok = pack.Init();
      if (!ok) return Prompt.PrintCannotContinue();

      ok = svn.Ignore("packages");
      if (!ok) return Prompt.PrintCannotContinue();

      return true;
    }
  }
}
