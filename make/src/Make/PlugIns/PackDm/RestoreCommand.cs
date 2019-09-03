using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Subversion;
using Make.Library.PackDm;
using Make.Library.NuGet;

namespace Make.PlugIns.PackDm
{
  class RestoreCommand : ICommand<RestoreCommandOptions>
  {
    public bool Exec(RestoreCommandOptions options)
    {
      bool ok;

      Toolchain.RequirePackDm(true);
      Toolchain.RequireSubversion(true);
      Toolchain.RequireNuGet(true);

      var pack = new Library.PackDm.PackDm();
      pack.PackConf = options.ConfFile.Text;
      pack.PackInfo = options.PackFile.Text;

      var svn = new Svn();
      var nuget = new NuGet();

      ok = pack.Install();
      if (!ok) return Prompt.PrintCannotContinue();

      ok = nuget.Restore();
      if (!ok) return Prompt.PrintCannotContinue();

      ok = svn.Ignore("packages");
      
      return true;
    }
  }
}
