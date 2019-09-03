using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Posix;

namespace Make.PlugIns.Core
{
  class UpgradeToolchainCommand : ICommand<UpgradeToolchainCommandOptions>
  {
    public bool Exec(UpgradeToolchainCommandOptions options)
    {
      Toolchain.RequireAll(force: true);
      return true;
    }
  }
}
