using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

using Do.Library.Posix;
using Do.Library.PackDm;

namespace Do.PlugIns.PackDm
{
  [Command]
  class DeployCommand
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
