using System;
using System.IO;
using PackDm.Handlers;
using PackDm.Model;

namespace PackDm
{
  public static class ContextFactory
  {
    public static Context Create(Options options)
    {
      var conf = CreateConf(options);
      var fileSystem = CreateFileSystem(options, conf);

      var settings = new Settings(conf, options);

      var packFile = fileSystem.PackFile;
      var pack = packFile.Exists ? PackHandler.Load(packFile) : new Pack();

      var credentialsFile = settings.CredentialsFile;
      var credentials = File.Exists(credentialsFile) ? CredentialsHandler.Load(credentialsFile) : new Credentials();

      return new Context
      {
        Pack = pack,
        Options = options,
        Settings = settings,
        FileSystem = fileSystem,
        Credentials = credentials
      };
    }

    private static Conf CreateConf(Options options)
    {
      FileInfo confFile = null;

      if (options.ConfFileOn)
      {
        confFile = new FileInfo(options.ConfFileValue);
      }
      else
      {
        var defaultFileSystem = new FileSystem();
        confFile = defaultFileSystem.ConfFile;
      }

      var conf = confFile.Exists ? ConfHandler.Load(confFile) : new Conf();
      return conf;
    }

    private static FileSystem CreateFileSystem(Options options, Conf conf)
    {
      var fileSystem = new FileSystem();

      if (conf.GetValue(Conf.PackFileKey) != null)
      {
        fileSystem.PackFile = new FileInfo(conf.GetValue(Conf.PackFileKey));
      }
      if (conf.GetValue(Conf.DistFolderKey) != null)
      {
        fileSystem.DistFolder = new DirectoryInfo(conf.GetValue(Conf.DistFolderKey));
      }
      if (conf.GetValue(Conf.DepsFolderKey) != null)
      {
          fileSystem.DepsFolder = new DirectoryInfo(conf.GetValue(Conf.DepsFolderKey));
      }
      if (conf.GetValue(Conf.DistFileKey) != null)
      {
        var distFolder = fileSystem.DistFolder.FullName;
        var distFile = Path.Combine(distFolder, conf.GetValue(Conf.DistFileKey));
        fileSystem.DistFile = new FileInfo(distFile);
      }

      // definindo pastas
      //
      if (options.DistFolderOn)
      {
        fileSystem.DistFolder = new DirectoryInfo(options.DistFolderValue);
      }
      if (options.DepsFolderOn)
      {
        fileSystem.DepsFolder = new DirectoryInfo(options.DepsFolderValue);
      }

      // definindo arquivos
      //
      if (options.PackPrefixOn)
      {
        var prefix = options.PackPrefixValue;
        var confFileName = prefix + "." + Settings.DefaultConfFile;
        var packFileName = prefix + "." + Settings.DefaultPackFile;
        var distFileName = prefix + "." + Settings.DefaultDistFile;

        var distFolder = fileSystem.DistFolder.FullName;
        distFileName = Path.Combine(distFolder, distFileName);

        fileSystem.ConfFile = new FileInfo(confFileName);
        fileSystem.PackFile = new FileInfo(packFileName);
        fileSystem.DistFile = new FileInfo(distFileName);
      }
      if (options.ConfFileOn)
      {
        fileSystem.ConfFile = new FileInfo(options.ConfFileValue);
      }
      if (options.PackFileOn)
      {
        fileSystem.PackFile = new FileInfo(options.PackFileValue);
      }
      if (options.DistFileOn)
      {
        var distFolder = fileSystem.DistFolder.FullName;
        var distFile = Path.Combine(distFolder, options.DistFileValue);
        fileSystem.DistFile = new FileInfo(distFile);
      }

      // opções boolianas
      //
      var booleanOptions = conf.GetValues(Conf.OptionsKey);
      foreach (var booleanOption in booleanOptions)
      {
        if (booleanOption == Conf.OptionsKey_FlatFolder)
        {
          fileSystem.FlatFolder = true;
        }
      }
      if (options.FlatFolderOn)
      {
        fileSystem.FlatFolder = true;
      }

      return fileSystem;
    }

  }
}

