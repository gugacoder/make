using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PackDm.Handlers;
using PackDm.Helpers;
using PackDm.Model;

namespace PackDm.Algorithms
{
  public class PackUpgrader
  {
    public void UpgradePackProject(Uri repositoryUri, FileInfo packFile, IEnumerable<Artifact> dependencies)
    {
      var pack = PackHandler.Load(packFile);

      var index = DownloadRepositoryIndex(repositoryUri);

      var resolver = new DependencyResolver();
      var artifacts = resolver.ResolveArtifacts(index, dependencies);

      var semver = new Semver();

      foreach (var artifact in artifacts)
      {
        var dep = pack.Deps.Find(a => a.HasSameRadical(artifact));
        if (dep == null)
          continue;

        var current = dep.Version;
        var upgrade = semver.UpgradePattern(dep.Version, null, artifact.Version);

        if (current != upgrade)
        {
          dep.Version = upgrade;
          Console.WriteLine($"[upgrade]{dep.Group}/{dep.Name} from {current} to {upgrade}");
        }
      }
      PackHandler.Save(pack, packFile);
    }

    private Model.Index DownloadRepositoryIndex(Uri repositoryUri)
    {
      var indexUri = new Uri(repositoryUri, "pack.index");
      var index = IndexHandler.Load(indexUri);
      return index;
    }
  }
}
