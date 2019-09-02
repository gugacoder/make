using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Make.Helpers;
using Make.Library.Subversion;

//
// ESTUDO EM ANDAMENTO
//
// Project irá substituir os vários algoritmos de captura dos
// parâmetros do projeto do PackDm e do Subversion, facilitando
// acrescentar posteriormente informações do GIT e fo NuGet.
//

namespace Make.Library.Projects
{
  /// <summary>
  /// O nome de pré-lançamento da versão, como alfa, beta, trunk, etc;
  /// </summary>
  class PreReleaseInfo
  {
    private string _name;
    private int? _revision;

    private PreReleaseInfo(string value)
    {
      this.Value = value.Trim();
    }

    /// <summary>
    /// O nome de pré-lançamento da versão, como alfa, beta, trunk, etc;
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// O nome de revisao do pré-release.
    /// Como o nome "beta" em:
    ///   2.5.3-beta1
    /// </summary>
    public string Name => _name ?? (_name = Regex.Replace(Value, @"(.*?)(\d+)?$", "$1"));

    /// <summary>
    /// O número de revisao do pré-release.
    /// Como o número 1 em:
    ///   2.5.3-beta1
    /// </summary>
    public int Revision
    {
      get
      {
        if (_revision == null)
        {
          var part = Regex.Replace(Value, @"(.*?)(\d+)?$", "$2");
          _revision = (int.TryParse(part, out int rev)) ? rev : 0;
        }
        return _revision.Value;
      }
    }

    public override string ToString()
    {
      return Value;
    }

    public static implicit operator PreReleaseInfo(string value)
    {
      return string.IsNullOrWhiteSpace(value) ? null : new PreReleaseInfo(value);
    }

    public static implicit operator string(PreReleaseInfo value)
    {
      return value?.Value;
    }
  }
}
