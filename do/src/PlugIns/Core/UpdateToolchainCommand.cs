using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Do.Library.Posix;

namespace Do.PlugIns.Core
{
  [Command]
  class UpgradeToolchainCommand
  {
    public bool Exec(UpgradeToolchainCommandOptions options)
    {
      Toolchain.RequireAll(force: true);
      return true;
    }
  }
}
