using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Do.Helpers
{
  /// <summary>
  /// Utilitário de geração de GUID determinístico.
  /// </summary>
  static class DeterministicGuid
  {
    /// <summary>
    /// Obtém um GUID específico para um texto determinado.
    /// O GUID será sempre o mesmo quando o texto for o mesmo.
    /// </summary>
    /// <param name="text">O texto para o qual se deseja obter o GUID.</param>
    /// <returns>O GUID específico para o texto.</returns>
    public static Guid GetDeterministicGuid(string text)
    {
      MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
      byte[] inputBytes = Encoding.Default.GetBytes(text);
      byte[] hashBytes = provider.ComputeHash(inputBytes);
      Guid hashGuid = new Guid(hashBytes);
      return hashGuid;
    }
  }
}
