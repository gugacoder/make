using System;
using System.Linq;
using PackDm.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PackDm.IO;

namespace PackDm.Algorithms
{
  public class PackBuilder
  {
    public Pack Pack
    {
      get { return pack ?? (pack = new Pack()); }
      set { pack = value; }
    }
    private Pack pack;

    public void ClonePropertiesFrom(Pack sourcePack)
    {
      Pack.Id = sourcePack.Id;

      Pack.Deps.Clear();
      Pack.Deps.AddRange(sourcePack.Deps.Select(d => (Artifact)d.Id));

      Pack.Dist.Clear();
      Pack.Dist.AddRange(sourcePack.Dist);
      Pack.NormalizeDistNames();
    }

    public void ResolveDependencies(Uri repositoryUri)
    {
      var indexResolver = new RepositoryIndexResolver();
      var index = indexResolver.ResolveIndex(repositoryUri);

      var resolver = new DependencyResolver();
      var dependencies = resolver.ResolveArtifacts(index, Pack.Deps);

      Pack.Deps.Clear();
      Pack.Deps.AddRange(dependencies);
    }

    public void ResolveDistribution(FileSystem fileSystem)
    {
      var resolver = new DistributionResolver();
      var distribution = resolver.ResolveDistribution(fileSystem, Pack);

      Pack.Dist.Clear();
      Pack.Dist.AddRange(distribution);
      Pack.NormalizeDistNames();
    }

    public void SetVersion(string versionMask)
    {
      var version = Pack.Version;

      if (!version.HasMajor)
        version.MajorInt32 = 0;
      if (!version.HasMinor)
        version.MinorInt32 = 0;
      if (!version.HasPatch)
        version.PatchInt32 = 0;

      if (!string.IsNullOrWhiteSpace(versionMask))
      {
        version = MergeVersion(pack.Version, versionMask);
      }

      if (!version.IsValid)
      {
        throw new PackDmException("Nada foi feito. A versão do pacote não é válida: " + pack.Version);
      }

      Pack.Version = version;
    }

    private Tag MergeVersion(Tag version, Tag mask)
    {
      var tag = new Tag{ Version = version };

      if (mask.HasMajor && (mask.Major != "*" && mask.Major != "x"))
        tag.Major = mask.Major;

      if (mask.HasMinor && (mask.Minor != "*" && mask.Minor != "x"))
        tag.Minor = mask.Minor;

      if (mask.HasPatch && (mask.Patch != "*" && mask.Patch != "x"))
        tag.Patch = mask.Patch;

      if (mask.PreRelease != "*" && mask.PreRelease != "x")
        tag.PreRelease = mask.PreRelease;

      return tag;
    }

  }
}