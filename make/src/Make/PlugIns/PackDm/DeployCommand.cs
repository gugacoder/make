using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Make.Library.Posix;
using Make.Library.PackDm;

namespace Make.PlugIns.PackDm
{
  class DeployCommand : ICommand<DeployCommandOptions>
  {
    public bool Exec(DeployCommandOptions options)
    {
      bool ok;

      Toolchain.RequirePackDm(true);

      var pack = new Library.PackDm.PackDm();
      pack.PackConf = options.PackConf.Text;
      pack.PackInfo = options.PackInfo.Text;

      ok = pack.Deploy();
      if (!ok) return Prompt.PrintCannotContinue();

      return true;
    }
  }
}
