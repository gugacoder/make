using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PackDm.IO;
using PackDm.Model;
using PackDm.Handlers;

namespace PackDm.Algorithms
{
  public class RepositoryIndexResolver
  {
    private static Dictionary<string, Index> indexes;
    private static readonly object synclock = new object();

    static RepositoryIndexResolver()
    {
      indexes = new Dictionary<string, Index>();
    }

    public Index ResolveIndex(Uri uri)
    {
      lock (synclock)
      {
        var key = uri.AbsoluteUri;
        if (indexes.ContainsKey(key))
          return indexes[key];

        var index = DownloadRepositoryIndex(uri);
        indexes[key] = index;
        return index;
      }
    }

    private Index DownloadRepositoryIndex(Uri uri)
    {
      var indexUri = new Uri(uri, "pack.index");
      var index = IndexHandler.Load(indexUri);
      return index;
    }

  }
}