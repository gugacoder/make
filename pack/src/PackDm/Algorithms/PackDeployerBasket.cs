using System;
using System.IO;
using PackDm.Model;
using System.Collections.Generic;
using PackDm.IO;

namespace PackDm.Algorithms
{
  public class PackDeployerBasket : IDisposable
  {
    private readonly Uploader uploader;
    private readonly Queue<Resource> resources;

    private class Resource
    {
      public FileInfo SourceFile { get; set; }
      public Uri TargetUri { get; set; }
      public bool IsUploaded { get; set; }
    }

    public PackDeployerBasket()
    {
      this.uploader = new Uploader();
      this.resources = new Queue<Resource>();
    }

    public void Add(FileInfo sourceFile, Uri targetUri)
    {
      var resource = new Resource
      {
        SourceFile = sourceFile,
        TargetUri = targetUri
      };
      resources.Enqueue(resource);
    }

    public void Complete()
    {
      UploadFiles();
    }

    private void UploadFiles()
    {
      foreach (var resource in resources)
      {
        UploadResource(resource);
      }
    }

    private void UploadResource(Resource resource)
    {
      if (!resource.IsUploaded)
      {
        var uri = resource.TargetUri;
        var filepath = resource.SourceFile.FullName;

        uploader.UploadFile(uri, filepath);

        resource.IsUploaded = true;
      }
    }

    public void Dispose()
    {
    }
  }
}

