using System;
using PackDm.Model;
using System.Net;
using System.IO;

namespace PackDm.IO
{
  public class Uploader
  {
    public void UploadFile(Uri uri, string filename)
    {
      try
      {
        Console.WriteLine("[up][from]" + filename);
        Console.WriteLine("[up][to]" + uri.AbsoluteUri);

        using (var input = new FileStream(filename, FileMode.Open))
        {
          using (var output = CreateOutputStream(uri))
          {
            input.CopyTo(output);
            output.Flush();
          }
        }

        Console.WriteLine("[up][ok]" + uri.AbsoluteUri);

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

    private Stream CreateOutputStream(Uri uri)
    {
      if (uri.IsFile)
      {
        var file = new FileInfo(uri.AbsolutePath);
        var directory = file.Directory;

        if (!directory.Exists)
          directory.Create();

        var stream = file.OpenWrite();
        return stream;
      }
      else
      {
        var web = WebClientFactory.Current.CreateWebClient(uri);
        web.Headers["Content-Type"] = "application/octet-stream";
        var stream = web.OpenWrite(uri.CreateNoCachedUriVersion());
        return stream;
      }
    }

  }
}

