using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Linq;
using PackDm.Helpers;

namespace PackDm.Model
{
  public class Index : SortedSet<Artifact>
  {
    private static readonly Semver semver = new Semver();
    private readonly Dictionary<string, List<Artifact>> dependencies;

    public Index()
    {
      this.dependencies = new Dictionary<string, List<Artifact>>();
    }

    public IEnumerable<Artifact> FindMatches(Artifact targetArtifact)
    {
      var matches = FindMatches(this, targetArtifact);
      return matches;
    }

    public IEnumerable<Artifact> FindDependencies(Artifact targetArtifact)
    {
      if (this.dependencies.ContainsKey(targetArtifact.Id))
      {
        var references = this.dependencies[targetArtifact.Id];
        return references;
      }
      return Enumerable.Empty<Artifact>();
    }

    public void RegisterConstraint(Artifact artifact, Artifact dependency)
    {
      List<Artifact> references = null;
      if (this.dependencies.ContainsKey(artifact.Id))
      {
        references = this.dependencies[artifact.Id];
      }
      else
      {
        references = new List<Artifact>();
        this.dependencies.Add(artifact.Id, references);
      }

      references.RemoveAll(d => d.Group == dependency.Group && d.Name == dependency.Name);
      references.Add(dependency);
    }

    public static IEnumerable<Artifact> FindMatches(IEnumerable<Artifact> artifactCollection, Artifact targetArtifact)
    {
      var artifacts =
        from a in artifactCollection
        where a.Group == targetArtifact.Group
           && a.Name == targetArtifact.Name
        select a;

      var versions = artifacts.Select(x => x.Version);
      var matchVersions = semver.FindMatches(versions, targetArtifact.Version);

      var matches = artifacts.Where(a => matchVersions.Contains(a.Version));
      return matches;
    }

  }
}

