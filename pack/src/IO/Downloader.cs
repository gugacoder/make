using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using PackDm.Model;

namespace PackDm.IO
{
  public class Downloader
  {
    public void DownloadFile(Uri uri, string filename)
    {
      try
      {
        Console.WriteLine("[down][uri]" + uri.AbsoluteUri);
        Console.WriteLine("[down][to]" + filename);

        var web = WebClientFactory.Current.CreateWebClient(uri);
        web.DownloadFile(uri.CreateNoCachedUriVersion(), filename);

        Console.WriteLine("[down][ok]" + uri.AbsoluteUri);
      }
      catch (Exception ex)
      {
        HttpWebResponse response = null;
        if (ex is WebException)
        {
          response = ((WebException)ex).Response as HttpWebResponse;
        }

        if (response != null)
        {
          var status = response.StatusCode;
          Console.WriteLine("[up][" + status + "]" + uri.AbsoluteUri);
        }
        else if (ex.Message.Contains("timed out")
          || ex.Message.Contains("Timed out")
          || ex.Message.Contains("timeout")
          || ex.Message.Contains("Timeout"))
        {
          Console.WriteLine("[up][timeout]" + uri.AbsoluteUri);
        }
        else
        {
          Console.WriteLine("[up][err]" + ex.Message);
        }

        throw ex;
      }
    }
  }
}

