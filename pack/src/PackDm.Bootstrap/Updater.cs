using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Reflection;

namespace PackDm.Bootstrap
{
  public class Updater
  {
    /// <summary>
    /// Tempo padrão para espera por resposta do servidor remoto em segundos.
    /// </summary>
    public const int DefaultTimeout = 0;

    public static readonly Uri[] DefaultUris = {
      new Uri("http://keepcoding.net/pack/updater/")
    };

    public void UpdatePackDm(Options options)
    {
      var uris = new List<Uri>();
      if (options.CustomUriOn)
      {
        var customUri = options.CustomUriValue;
        if (!customUri.EndsWith("/"))
        {
          customUri += "/";
        }
        uris.Add(new Uri(customUri));
      }
      uris.AddRange(DefaultUris);

      foreach (var uri in uris)
      {
        var ok = UpdateFiles(options, uri);
        if (ok)
        {
          return;
        }
      }

      Console.WriteLine("Não foi possível baixar os binários a partir das URIs conhecidas.");
    }

    private bool UpdateFiles(Options options, Uri baseUri)
    {
      var bootstrapFile = "pack-bootstrap.exe";
      var packFile = "pack.exe";

      var bootstrapUri = new Uri(baseUri, bootstrapFile);
      var packUri = new Uri(baseUri, packFile);

      bool ok = false;
      if (options.SelfOn)
      {
        ok = UpdateFile(options, bootstrapUri, bootstrapFile, true);
      }
      else
      {
        ok = UpdateFile(options, packUri, packFile, false);
      }
      return ok;
    }

    private bool UpdateFile(Options options, Uri remoteUri, string targetFile, bool keepBackup)
    {
      Console.WriteLine("[uri]" + remoteUri);

      var web = new CustomWebClient();
      web.Timeout = GetTimeout(options);

      var tempFile = Path.GetTempFileName();
      var backupFile = targetFile + ".previous";
      
      try
      {
        web.DownloadFile(remoteUri.CreateNoCachedUriVersion(), tempFile);

        if (File.Exists(targetFile))
        {
          if (keepBackup)
          {
            if (File.Exists(backupFile))
            {
              File.Delete(backupFile);
            }
            File.Move(targetFile, backupFile);
            File.SetAttributes(backupFile, FileAttributes.Hidden);
          }
          else
          {
            File.Delete(targetFile);
          }
        }

        File.Move(tempFile, targetFile);
        
        Console.WriteLine("[ok]" + remoteUri);
        return true;

      }
      catch (Exception ex)
      {
        try
        {
          File.Move(backupFile, targetFile);
        }
        catch { /* nada a fazer */ }

        HttpWebResponse response = null;

        if (ex is WebException)
        {
          response = ((WebException)ex).Response as HttpWebResponse;
        }
        
        if (response != null)
        {
          var status = response.StatusCode;
          Console.WriteLine("[" + status + "]" + remoteUri);
        }
        else if (ex.Message.Contains("timed out")
              || ex.Message.Contains("Timed out")
              || ex.Message.Contains("timeout")
              || ex.Message.Contains("Timeout"))
        {
          Console.WriteLine("[timeout]" + remoteUri);
        }
        else
        {
          Console.WriteLine("[err]" + remoteUri);
          Program.DumpException(ex, options.VerboseOn);
        }
      }
      finally
      {
        try
        {
          if (File.Exists(tempFile)) File.Delete(tempFile);
        }
        catch { /* nada a fazer */ }
        try
        {
          if (File.Exists(backupFile)) File.Delete(backupFile);
        }
        catch { /* nada a fazer */ }
      }

      return false;
    }

    /// <summary>
    /// Obtém o tempo de espera por resposta do servidor remoto em segundos,
    /// considerando as configurações do sistema e de linha de comando.
    /// </summary>
    /// <returns>O tempo de espera sem sgundos.</returns>
    /// <param name="options">Os argumentos de linha de comando.</param>
    public int GetTimeout(Options options)
    {
      int timeout = DefaultTimeout;

      if (options.TimeoutOn)
      {
        var ok = int.TryParse(options.TimeoutValue, out timeout);
        if (!ok)
        {
          timeout = DefaultTimeout;
        }
      }

      return timeout;
    }

  }
}
