using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.PlugIns.Core
{
  [Command]
  class GuidCommand
  {
    public bool Exec(GuidCommandOptions options)
    {
      var format = options.Format.Text ?? "B";
      var count = options.Count.On ? options.Count.Value : 1;
      while (count-- > 0)
      {
        Prompt.WriteInfo(Guid.NewGuid().ToString(format).ToUpper());
      }
      return true;
    }
  }
}
