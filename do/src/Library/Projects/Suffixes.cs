using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

//
// ESTUDO EM ANDAMENTO
//
// Project irá substituir os vários algoritmos de captura dos
// parâmetros do projeto do PackDm e do Subversion, facilitando
// acrescentar posteriormente informações do GIT e fo NuGet.
//

namespace Do.Library.Projects
{
  /// <summary>
  /// Opções de detalhamento da versão impressa.
  /// </summary>
  [Flags]
  public enum Suffixes
  {
    /// <summary>
    /// Nenhum sufixo adicional.
    /// </summary>
    None = 0,

    /// <summary>
    /// Acrescenta o sufixo de pré-release, se existir.
    /// </summary>
    PreRelease = 1,

    /// <summary>
    /// Acrescenta o sufixo de revisão do código fonte, se existir.
    /// </summary>
    Revision = 2,

    /// <summary>
    /// Acrescenta todos os sufixos.
    /// </summary>
    All = PreRelease | Revision
  }
}