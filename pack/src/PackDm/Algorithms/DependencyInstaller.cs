using System;
using System.Linq;
using PackDm.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PackDm.IO;
using PackDm.Handlers;

namespace PackDm.Algorithms
{
  public class DependencyInstaller
  {
    /// <summary>
    /// Quando ativado as dependências baixadas são instaladas diretamente
    /// na pasta Deps, em vez da tradicional estrutura "Grupo\Artefato".
    /// </summary>
    public bool FlatFolder { get; set; }

    public void InstallDependencies(Uri repositoryUri, DirectoryInfo depsFolder, IEnumerable<Artifact> dependencies)
    {
      var index = DownloadRepositoryIndex(repositoryUri);
      
      var resolver = new DependencyResolver();
      var artifacts = resolver.ResolveArtifacts(index, dependencies);

      DownloadArtifacts(repositoryUri, depsFolder, artifacts);
    }

    private Model.Index DownloadRepositoryIndex(Uri repositoryUri)
    {
      var indexUri = new Uri(repositoryUri, "pack.index");
      var index = IndexHandler.Load(indexUri);
      return index;
    }

    private void DownloadArtifacts(Uri repositoryUri, DirectoryInfo depsFolder, IEnumerable<Artifact> artifacts)
    {
      var depsPath = depsFolder.FullName;
      foreach (var artifact in artifacts)
      {
        using (var basket = new DependencyInstallerBasket())
        {
          // Contém grupo, nome e versao e segue o formato de URL.
          basket.CloudPath = new Uri(repositoryUri, artifact.Id + "/");

          // O caminho local, por padrão, contém grupo e nome e segue o formato de pasta.
          // Quando FlatFolder é ativado o grupo e o nome não são usados.
          if (FlatFolder)
          {
            var targetName =
              artifact.Group + "." + artifact.Name + "." + Settings.DefaultPackFile;

            basket.LocalPath = new DirectoryInfo(depsPath);
            basket.Add(Settings.DefaultPackFile, targetName);
          }
          else
          {
            basket.LocalPath = new DirectoryInfo(Path.Combine(depsPath, artifact.Group, artifact.Name));
            basket.Add(Settings.DefaultPackFile);
          }

          using (var reader = basket.CreateReader(Settings.DefaultPackFile))
          {
            var pack = PackHandler.Load(reader);

            foreach (var distname in pack.Dist)
            {
              basket.Add(distname);
            }
          }

          basket.Complete();
        }
      }
    }

  }
}

