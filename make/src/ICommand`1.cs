using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make
{
  interface ICommand<TOptions> : ICommand
  {
    bool Exec(TOptions options);
  }
}
