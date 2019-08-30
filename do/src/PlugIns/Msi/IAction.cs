using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.PlugIns.Msi
{
  interface IAction
  {
    bool Exec(MsiCommandOptions options);
  }
}
