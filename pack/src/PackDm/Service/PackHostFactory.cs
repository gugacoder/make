using System;
using PackDm.Model;
using System.IO;

namespace PackDm.Service
{
  public static class PackHostFactory
  {
    public static PackHost CreateFileHost(Settings settings, Options options)
    {
      var folder = settings.RepositoryFolder;
      var port = settings.Port;
      var host = new PackHost(port, folder);
      return host;
    }

  }
}

