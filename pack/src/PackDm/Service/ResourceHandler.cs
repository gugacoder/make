using System;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using PackDm.Model;
using System.Net;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using PackDm.Actions;
using System.Threading.Tasks;
using PackDm.Algorithms;
using System.Reflection;

namespace PackDm.Service
{
  public class ResourceHandler
  {
    public class ResourceInfo
    {
      public string Resource { get; set; }
      public bool IsFolder { get; set; }
      public bool IsFile { get; set; }
      public bool IsStaticContent { get; set; }
      public bool IsNotFound { get; set; }
      public bool IsRedirect { get; set; }
      public bool IsCacheable { get; set; }
      public string Path { get; set; }
      public string Location { get; set; }
    }

    public class OpenResourceInfo
    {
      public string Name { get; set; }
      public long Length { get; set; }
      public string MimeType { get; set; }
      public Stream Stream { get; set; }
    }

    private readonly DirectoryInfo repository;
    private readonly ResourceMapper mapper;
    private readonly RepositoryIndexer indexer;

    private readonly string[] staticFiles =
    {
      "index.xsl",
      "index.css"
    };

    public ResourceHandler(DirectoryInfo repository)
    {
      this.repository = repository;
      this.mapper = new ResourceMapper(repository);
      this.indexer = new RepositoryIndexer(repository);
    }

    public void UpdateIndex()
    {
      indexer.UpdateIndex();
    }

    public ResourceInfo GetResourceInfo(string resource)
    {
      var name = resource;
      if (name.EndsWith("/"))
        name = name.Substring(0, name.Length - 1);
      if (name.StartsWith("/"))
        name = name.Substring(1);

      var staticFile = staticFiles.FirstOrDefault(x => name.EndsWith(x));
      if (staticFile != null)
      {
        return new ResourceInfo
        {
          Resource = resource,
          IsStaticContent = true,
          IsCacheable = true,
          Path = staticFile
        };
      }

      var path =
        Path.Combine(
          repository.FullName,
          name.Replace('/', Path.DirectorySeparatorChar)
        );
      
      if (File.Exists(path))
      {
        return new ResourceInfo
        {
          Resource = resource,
          IsFile = true,
          Path = path
        };
      }

      if (Directory.Exists(path))
      {
        if (resource == "" || resource.EndsWith("/"))
        {
          return new ResourceInfo
          {
            Resource = resource,
            IsFolder = true,
            Path = path
          };
        }
        else
        {
          var location = name.Split('/').Last() + "/";
          return new ResourceInfo
          {
            Resource = resource,
            IsRedirect = true,
            Location = location
          };
        }
      }

      return new ResourceInfo
      {
        Resource = resource,
        IsNotFound = true,
        Path = path
      };
    }

    public OpenResourceInfo OpenResource(ResourceInfo info)
    {
      if (info.IsStaticContent)
      {
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream(info.Path);
        return new OpenResourceInfo
        {
          Name = info.Path,
          MimeType = MimeTypes.GetMimeType(info.Path),
          Stream = stream
        };
      }

      if (info.IsFile)
      {
        var file = new FileInfo(info.Path);
        var stream = file.OpenRead();
        return new OpenResourceInfo
        {
          Name = file.Name,
          Length = file.Length,
          MimeType = MimeTypes.GetMimeType(file),
          Stream = stream
        };
      }

      if (info.IsFolder)
      {
        var folder = mapper.MapResource(info.Resource);
        var stream = ResourceSerializer.ToStream(folder);
        return new OpenResourceInfo
        {
          Name = "index.xml",
          Length = 0,
          MimeType = "application/xml",
          Stream = stream
        };
      }

      throw new PackDmException("Uso incorreto do método OpenResource(ResourceInfo).");
    }

    public void SaveResource(ResourceInfo info, Stream content)
    {
      if (!(info.IsNotFound || info.IsFile))
        throw new PackDmException("Uso incorreto do método SaveResource(ResourceInfo, Stream).");

      var file = new FileInfo(info.Path);
      if (!file.Directory.Exists)
        file.Directory.Create();

      using (var stream = new FileStream(file.FullName, FileMode.Create))
      {
        content.CopyTo(stream);
        stream.Flush();
      }

      // A indexação não deveria ser automática mas sim executada
      // explicitamente depois de todos os recursos terem sido
      // modificados.
      //new Task(indexer.UpdateIndex).Start();
    }

    public void DeleteResource(ResourceInfo info)
    {
      if (info.IsFile)
      {
        File.Delete(info.Path);

        // A indexação não deveria ser automática mas sim executada
        // explicitamente depois de todos os recursos terem sido
        // modificados.
        //new Task(indexer.UpdateIndex).Start();
        return;
      }

      if (info.IsFolder)
      {
        Directory.Delete(info.Path, true);

        // A indexação não deveria ser automática mas sim executada
        // explicitamente depois de todos os recursos terem sido
        // modificados.
        //new Task(indexer.UpdateIndex).Start();
        return;
      }

      throw new PackDmException("Uso incorreto do método SaveResource(ResourceInfo, Stream).");
    }

  }
}

