using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PackDm.Model;
using PackDm.Handlers;

namespace PackDm.Algorithms
{
  public class PackCompiler
  {
    public bool ResolveDependenciesEnabled
    {
      get;
      set;
    }

    public bool ResolveDistributionEnabled
    {
      get;
      set;
    }

    public Uri SourceUri
    {
      get;
      set;
    }

    public FileSystem FileSystem
    {
      get { return fileSystem ?? (fileSystem = new FileSystem()); }
      set{ fileSystem = value; }
    }
    private FileSystem fileSystem;

    public FileInfo CompilePack(Pack pack, string versionMask)
    {
      var distPack = CreatePack(pack, versionMask);
      var packFile = SavePack(distPack);
      return packFile;
    }

    private Pack CreatePack(Pack pack, string versionMask)
    {
      var builder = new PackBuilder();
      builder.ClonePropertiesFrom(pack);

      if (ResolveDependenciesEnabled)
      {
        builder.ResolveDependencies(SourceUri);
      }

      if (ResolveDistributionEnabled)
      {
        builder.ResolveDistribution(FileSystem);
      }

      builder.SetVersion(versionMask);

      return builder.Pack;
    }

    private FileInfo SavePack(Pack pack)
    {
      var targetFile = FileSystem.DistFile;
      PackHandler.Save(pack, new TargetWriter(targetFile));
      return targetFile;
    }

  }
}

