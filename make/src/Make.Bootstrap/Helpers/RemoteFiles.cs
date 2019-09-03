using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Make.Bootstrap.Helpers
{
  static class RemoteFiles
  {
    public static string ReadText(string virtualPath)
    {
      var address = Settings.BaseUri + virtualPath;
      try
      {
        var client = new WebClient();
        var content = client.DownloadString(address);
        return content;
      }
      catch (Exception ex)
      {
        throw new IOException($"Falhou a tentativa de obter um arquivo texto: {address}", ex);
      }
    }

    public static void CopyFile(string virtualPath, string systemPath)
    {
      var address = Settings.BaseUri + virtualPath;
      try
      {
        var client = new WebClient();
        client.DownloadFile(address, systemPath);
      }
      catch (Exception ex)
      {
        throw new IOException($"Falhou a tentativa de baixar um arquivo: {address}", ex);
      }
    }
  }
}
