using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Make.Library.Projects;

namespace Make.Helpers
{
  /// <summary>
  /// Utilitário de leitura de um arquivo REVISION.txt que contém
  /// informação de versão do aplicativo.
  /// 
  /// Geralmente o arquivo contém uma única linha na forma:
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
  class RevisionFileLoader
  {
    public static VersionInfo GetRevisionInfo(string filepath = "REVISION.txt")
    {
      filepath = Path.GetFullPath(filepath);
      var content = GetContent(filepath);
      var revisionInfo = VersionInfo.Parse(content);
      return revisionInfo;
    }

    private static string GetContent(string filepath)
    {
      try
      {
        if (!File.Exists(filepath))
        {
          return "SNAPSHOT";
        }

        var lines =
          from line in File.ReadAllLines(filepath)
          let statement = line.Split('#').First().Trim()
          where !string.IsNullOrEmpty(statement)
          select statement;

        // O arquivo de versão deve conter apenas uma linha.
        // Mas se houverem mais, a informação das linhas subsequentes
        // serão consideradas adicionais.
        var version = string.Join("_", lines);

        return string.IsNullOrWhiteSpace(version) ? "SNAPSHOT" : version;
      }
      catch
      {
        return "SNAPSHOT";
      }
    }

    private static int ToInt(string text)
    {
      return int.TryParse(text, out int number) ? number : 0;
    }
  }
}
