using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Bootstrap.Commands
{
  interface ICommand
  {
    int Run(Action<string> logger);
  }
}
