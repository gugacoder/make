using System;
using System.Linq;
using PackDm.Model;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace PackDm.Model
{
  public class Settings
  {
    public const int DefaultPort = 8585;

    public const string DefaultConfFile = "pack.conf";
    public const string DefaultPackFile = "pack.info";
    public const string DefaultDistFile = "pack.info";
    public const string DefaultDepsFolder = "Deps";
    public const string DefaultDistFolder = "Dist";

    public static string[] DefaultSourceUri = new []
    {
      "http://keepcoding.net/pack/repository/"
    };

    private Conf conf;
    private Options options;

    public Settings(Conf conf, Options options)
    {
      this.conf = conf;
      this.options = options;
    }

    public static string DefaultFolder
    {
      get
      {
        var homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var folder = Path.Combine(homePath, ".pack", "repository");
        return folder;
      }
    }

    public int Port
    {
      get
      {
        var port = options.PortOn ? options.PortValue : conf.GetValue(Conf.PortKey);
        if (port != null)
        {
          try
          {
            var number = int.Parse(port);
            return number;
          }
          catch (Exception ex)
          {
            throw new PackDmException("A porta indicada não é válida: " + port, ex);
          }
        }
        return DefaultPort;
      }
    }

    public Uri[] RepositoryUris
    {
      get
      {
        if (repositoryUris == null)
        {
          var uris = conf.GetValues(Conf.SourceUriKey) ?? DefaultSourceUri;
          var urisCorrigidas =
            from uri in uris
            let uriCorrigida = (uri.EndsWith("/") ? uri : (uri + "/"))
            select new Uri(uriCorrigida);

          repositoryUris = urisCorrigidas.ToArray();
        }
        return repositoryUris;
      }
      set { repositoryUris = value; }
    }
    private Uri[] repositoryUris;

    public string RepositoryFolder
    {
      get
      {
        var folder =
          options.FolderOn
            ? options.FolderValue
            : (conf.GetValue(Conf.FolderKey) ?? DefaultFolder);

        folder = folder.Replace('/', Path.DirectorySeparatorChar);
        folder = folder.Replace('\\', Path.DirectorySeparatorChar);

        if (folder.Contains("{userdir}"))
        {
          var userdir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
          folder = folder.Replace("{userdir}", userdir);
        }

        if (folder.Contains("{appdir}"))
        {
          var appdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
          folder = folder.Replace("{appdir}", appdir);
        }

        if (folder.Contains("{curdir}"))
        {
          var curdir = Directory.GetCurrentDirectory();
          folder = folder.Replace("{curdir}", curdir);
        }

        return !string.IsNullOrWhiteSpace(folder) ? folder.Trim() : DefaultFolder;
      }
    }

    public Uri Proxy
    {
      get
      {
        try
        {
          var uri = conf.GetValue(Conf.ProxyKey);
          if (uri != null)
          {
            return new Uri(uri);
          }
        }
        catch { /* nada a fazer... */ }

        return null;
      }
    }

    public Uri ProxyHttps
    {
      get
      {
        try
        {
          var uri = conf.GetValue(Conf.ProxyHttpsKey);
          if (uri != null)
          {
            return new Uri(uri);
          }
        }
        catch { /* nada a fazer... */ }

        return Proxy;
      }
    }

    public string CredentialsFile
    {
      get
      {
        var userdir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var filename = Path.Combine(userdir, ".pack/pack.auth");
        return filename;
      }
    }

  }
}

