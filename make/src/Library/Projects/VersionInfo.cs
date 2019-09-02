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
  /// Informação de versão de software.
  /// </summary>
  class VersionInfo
  {
    /// <summary>
    /// O primeiro número da versão.
    /// -   X.*.*
    /// </summary>
    public int Major { get; set; }

    /// <summary>
    /// O número do meio da versão.
    /// -   *.X.*
    /// </summary>
    public int Minor { get; set; }

    /// <summary>
    /// O último número da versão.
    /// -   *.*.X
    /// </summary>
    public int Patch { get; set; }

    /// <summary>
    /// O nome de pré-lançamento da versão, como alfa, beta, trunk, etc;
    /// </summary>
    public PreReleaseInfo PreRelease { get; set; }

    /// <summary>
    /// O número de revisão do código fonte no repositório.
    /// </summary>
    public int Revision { get; set; }

    /// <summary>
    /// Porção dos números da versão formatados na forma: major.minor.patch
    /// </summary>
    public string FormattedNumber => string.Join(".", Major, Minor, Patch);

    /// <summary>
    /// Porção do pre-release formatado na forma: -prerelease
    /// </summary>
    public string FormattedPreRelease => (PreRelease != null) ? $"-{PreRelease}" : "";

    /// <summary>
    /// Porção da revisão formatado na forma: _rREV
    /// </summary>
    public string FormattedRevision => (Revision > 0) ? $"_r{Revision}" : "";

    /// <summary>
    /// Produz o nome completo da versão, na forma:
    /// -   X.X.X-sufixoX_rX
    /// Sendo:
    /// -   X
    ///         Um número qualquer.
    /// -   X.X.X
    ///         Obrigatório.
    ///         Número de versão do aplicativo.
    /// -   sufixoX
    ///         Opcional.
    ///         O nome da versão, como alfa, beta, trunk, etc.
    ///         Seguido opcionalmente de um número de revisão do sufixo.
    /// -   rX
    ///         Opcional.
    ///         O número de revisão no repositório de código fonte.
    /// </summary>
    public override string ToString()
    {
      return ToString(Suffixes.All);
    }

    /// <summary>
    /// Produz o nome completo da versão, na forma:
    /// -   X.X.X-sufixoX_rX
    /// Ou apenas a parte escolhida.
    /// Sendo:
    /// -   X
    ///         Um número qualquer.
    /// -   X.X.X
    ///         Obrigatório.
    ///         Número de versão do aplicativo.
    /// -   sufixoX
    ///         Opcional.
    ///         O nome da versão, como alfa, beta, trunk, etc.
    ///         Seguido opcionalmente de um número de revisão do sufixo.
    /// -   rX
    ///         Opcional.
    ///         O número de revisão no repositório de código fonte.
    /// </summary>
    /// <param name="parts">
    /// As partes extras de versão escolhidas.
    /// </param>
    public string ToString(Suffixes parts)
    {
      var text = FormattedNumber;
      if (parts.HasFlag(Suffixes.PreRelease))
      {
        text += FormattedPreRelease;
      }
      if (parts.HasFlag(Suffixes.Revision))
      {
        text += FormattedRevision;
      }
      return text;
    }

    /// <summary>
    /// Extrai informação de versão do texto indicado.
    /// 
    /// É esperado que o texto tenha a forma:
    /// -   X.X.X-sufixoX_rX
    /// Sendo:
    /// -   X
    ///         Um número qualquer.
    /// -   X.X.X
    ///         Obrigatório.
    ///         Número de versão do aplicativo.
    /// -   sufixoX
    ///         Opcional.
    ///         O nome da versão, como alfa, beta, trunk, etc.
    ///         Seguido opcionalmente de um número de revisão do sufixo.
    /// -   rX
    ///         Opcional.
    ///         O número de revisão no repositório de código fonte.
    /// </summary>
    /// <param name="version">O texto a ser analisado.</param>
    /// <returns>A informação de versão extraída.</returns>
    public static VersionInfo Parse(string version)
    {
      var instance = new VersionInfo();

      // É permitido usar o caracter # para comentar o texto.
      var lines =
        from line in version.Split('\n', '\r')
        let sentence = line.Split('#').First().Trim()
        where !string.IsNullOrEmpty(sentence)
        select sentence;
      version = string.Join("_", lines);

      // REGEX para o padrão: X.X.X-sufixo_rX
      var regex = new Regex(@"^(\d+)(?:\.(\d+))?(?:\.(\d+))?(?:-([a-zA-Z\d]+))?(?:_r(\d+))?");
      var match = regex.Match(version);
      if (match.Success)
      {
        instance.Major = int.TryParse(match.Groups[1].Value, out int major) ? major : 0;
        instance.Minor = int.TryParse(match.Groups[2].Value, out int minor) ? minor : 0;
        instance.Patch = int.TryParse(match.Groups[3].Value, out int patch) ? patch : 0;
        instance.PreRelease = match.Groups[4].Value;
        instance.Revision = int.TryParse(match.Groups[5].Value, out int rev) ? rev : 0;
      }
      return instance;
    }

    public static implicit operator VersionInfo(string version)
    {
      return Parse(version);
    }

    public static implicit operator string(VersionInfo version)
    {
      return version.ToString();
    }
  }
}
