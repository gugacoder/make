using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Do.Library.Shell;

namespace Do.Library.NuGet
{
  class NuGet
  {
    /// <summary>
    /// Restaura as dependências do projeto.
    /// </summary>
    /// <returns>Verdadeiro se bem sucedido, falso caso contrário.</returns>
    public bool Restore()
    {
      var ok = "nuget restore".Run();
      return ok;
    }
  }
}
