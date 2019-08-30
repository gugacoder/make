using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackDm.Bootstrap
{
  static class UriExtensions
  {
    private readonly static Random Random = new Random();

    /// <summary>
    /// Acrescenta um argumento aleatório na URI para evitar caching.
    /// </summary>
    public static Uri CreateNoCachedUriVersion(this Uri uri)
    {
      var version = Random.Next(int.MaxValue);
      var noCachedUri = uri.ToString();
      noCachedUri += (noCachedUri.Contains("?") ? "&" : "?") + "v=" + version;
      return new Uri(noCachedUri);
    }
  }
}
