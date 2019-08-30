﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.PlugIns.PackDm
{
  [Command]
  class UpgradeCommand
  {
    public bool Exec(UpgradeCommandOptions options)
    {
      bool ok;

      Toolchain.RequirePackDm(true);

      var pack = new Library.PackDm.PackDm();
      pack.PackConf = options.ConfFile.Text;
      pack.PackInfo = options.PackFile.Text;

      ok = pack.Upgrade();
      if (!ok) return Prompt.PrintCannotContinue();

      return true;
    }
  }
}
