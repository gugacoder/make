using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using PackDm.IO;
using PackDm.Model;

namespace PackDm.Algorithms
{
  public class DependencyInstallerBasket : IDisposable
  {
    private readonly Downloader downloader;
    private readonly Dictionary<string, Resource> resources;

    private bool complete;

    private class Resource
    {
      public Uri Uri { get; set; }
      public FileInfo TempPath { get; set; }
      public FileInfo FilePath { get; set; }
      public bool IsDownloaded { get; set; }
    }

    public DependencyInstallerBasket()
    {
      this.downloader = new Downloader();
      this.resources = new Dictionary<string, Resource>();
    }

    public Uri CloudPath
    {
      get;
      set;
    }

    public DirectoryInfo LocalPath
    {
      get;
      set;
    }

    public void Add(string filename)
    {
      Add(filename, filename);
    }

    public void Add(string filename, string targetName)
    {
      var resource = new Resource
      {
        Uri = new Uri(CloudPath, filename),
        TempPath = new FileInfo(Path.GetTempFileName()),
        FilePath = new FileInfo(Path.Combine(LocalPath.FullName, targetName))
      };
      resources.Add(filename, resource);
    }

    public SourceReader CreateReader(string filename)
    {
      if (resources.ContainsKey(filename))
      {
        var resource = resources[filename];

        DownloadResource(resource);

        if (resource.TempPath.Exists)
          return new SourceReader(resource.TempPath.FullName);
      
        if (resource.FilePath.Exists)
          return new SourceReader(resource.FilePath.FullName);
        
      }
      throw new Exception("Não foi possível obter o arquivo: " + filename);
    }

    public void Complete()
    {
      DownloadResources();
      MoveResourcesToDestination();
      complete = true;
    }

    private void DownloadResources()
    {
      foreach (var resource in resources.Values)
      {
        DownloadResource(resource);
      }
    }

    private void DownloadResource(Resource resource)
    {
      if (!resource.IsDownloaded)
      {
        downloader.DownloadFile(resource.Uri, resource.TempPath.FullName);
        resource.IsDownloaded = true;
      }
    }

    private void MoveResourcesToDestination()
    {
      foreach (var resource in resources.Values)
      {
        MoveResourceToDestination(resource);
      }
    }

    private void MoveResourceToDestination(Resource resource)
    {
      var folder = resource.FilePath.Directory;
      if (!folder.Exists)
      {
        folder.Create();
        Console.WriteLine("[move][mkdir]" + folder.FullName);
      }

      if (resource.FilePath.Exists)
      {
        resource.FilePath.Delete();
        Console.WriteLine("[move][delete]" + resource.FilePath.FullName);
      }

      Console.WriteLine("[move][from]" + resource.TempPath.FullName);
      resource.TempPath.MoveTo(resource.FilePath.FullName);
      Console.WriteLine("[move][to]" + resource.FilePath.FullName);

    }

    public void Dispose()
    {
      DestroyFiles();
    }

    private void DestroyFiles()
    {
      if (!complete)
      {
        var files = resources.Values.SelectMany(x => new []{ x.FilePath, x.TempPath });
        foreach (var file in files)
        {
          try
          {
            if (file.Exists)
              file.Delete();
          }
          catch
          {
            // nada a fazer...
          }
        }

        if (LocalPath.Exists && !LocalPath.EnumerateFiles().Any())
        {
          try
          {
            LocalPath.Delete();
          }
          catch
          {
            // nada a fazer...
          }
        }
      }
    }

  }
}

