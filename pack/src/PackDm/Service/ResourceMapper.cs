using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace PackDm.Service
{
  public class ResourceMapper
  {
    private DirectoryInfo repository;

    public ResourceMapper(DirectoryInfo repository)
    {
      this.repository = repository;
    }

    public Resource MapResource(string resource)
    {
      if (resource.EndsWith("/"))
        resource = resource.Substring(0, resource.Length - 1);
      if (resource.StartsWith("/"))
        resource = resource.Substring(1);

      var directoryPath = resource.Replace('/', Path.DirectorySeparatorChar);
      var directory = new DirectoryInfo(Path.Combine(repository.FullName, directoryPath));

      var graph = new Resource();
      graph.Name = resource.Split('/').Last();
      graph.Path = "/" + resource;
      graph.Date = directory.LastWriteTime;

      foreach (var subdirectory in directory.EnumerateDirectories())
      {
        var name = subdirectory.Name;
        var path = "/" + resource + "/" + name;

        var item = new Resource.Item();
        item.Name = name;
        item.Path = path;
        item.Date = subdirectory.LastWriteTime;
        item.LengthInBytes = 0;
        item.IsFolder = true;
        graph.Items.Add(item);
      }

      foreach (var file in directory.EnumerateFiles())
      {
        var name = file.Name;
        var path = "/" + resource + "/" + name;

        var item = new Resource.Item();
        item.Name = file.Name;
        item.Path = path;
        item.Date = file.LastWriteTime;
        item.LengthInBytes = file.Length;
        graph.Items.Add(item);
      }

      return graph;
    }
  }
}

