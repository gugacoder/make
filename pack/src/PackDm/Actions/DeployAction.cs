using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using PackDm.Algorithms;
using PackDm.Model;
using PackDm.IO;

namespace PackDm.Actions
{
  public class DeployAction : IAction
  {
    public void Proceed(Context context)
    {
      var settings = context.Settings;
      var fileSystem = context.FileSystem;

      Exception lastException = null;
      var success = false;
      var repositories = settings.RepositoryUris;

      // URIs realmente alcançadas durante o upload.
      var touchedRepositories = new List<Uri>();

      //
      // Publicando os artefatos
      //
      var deployer = new PackDeployer();
      foreach (var uri in repositories)
      {
        try
        {
          deployer.DeployPack(uri, fileSystem.DistFile, fileSystem.DistFolder);
          success = true;
          touchedRepositories.Add(uri);
          break;
        }
        catch (Exception ex)
        {
          lastException = ex;
        }
      }

      //
      // Indexando repositórios locais se necessário.
      //
      var localRepositories = touchedRepositories.Where(u => u.IsFile);
      var remoteRepositories = touchedRepositories.Where(u => !u.IsFile);
      if (localRepositories.Any())
      {
        foreach (var uri in localRepositories)
        {
          var directory = uri.AbsolutePath;
          var indexer = new RepositoryIndexer(directory);
          try
          {
            indexer.UpdateIndex();
          }
          catch (Exception ex)
          {
            Program.DumpException("Falhou a tentativa de indexar o repositorio: " + directory, ex);
          }
        }
      }
      if (remoteRepositories.Any())
      {
        var removeReindexer = new RemoteReindexer();
        foreach (var uri in remoteRepositories)
        {
          try
          {
            removeReindexer.Reindex(uri);
          }
          catch (Exception ex)
          {
            Program.DumpException("Falhou a tentativa de indexar o repositorio remoto: " + uri, ex);
          }
        }
      }

      if (!success)
        throw new Exception(lastException.Message, lastException);
    }
  }
}

