using System;
using System.IO;
using System.Net;
using PackDm.IO;

namespace PackDm
{
  public class TargetWriter : IDisposable
  {
    private event Action Disposed;
    private readonly TextWriter writer;

    public TargetWriter(FileInfo targetFile)
      : this(targetFile.FullName)
    {
      
    }

    public TargetWriter(string targetFile)
    {
      var file = new FileInfo(targetFile);
      if (!file.Directory.Exists)
      {
        file.Directory.Create();
      }

      var stream = new FileStream(file.FullName, FileMode.Create);
      this.Disposed += () => stream.Dispose();

      this.writer = new StreamWriter(stream);
    }

    public TargetWriter(Uri target)
    {
      var web = WebClientFactory.Current.CreateWebClient(target);

      var stream = web.OpenWrite(target.CreateNoCachedUriVersion());
      this.Disposed += () => stream.Dispose();

      this.writer = new StreamWriter(stream);
    }

    public TargetWriter(Stream target)
    {
      this.writer = new StreamWriter(target);
    }

    public TargetWriter(TextWriter target)
    {
      this.writer = target;
    }

    public TextWriter GetWriter()
    {
      return writer;
    }

    public void Dispose()
    {
      if (Disposed != null)
      {
        Disposed.Invoke();
      }
    }

    public static implicit operator TextWriter(TargetWriter target)
    {
      return target.writer;
    }

    public static implicit operator TargetWriter(TextWriter target)
    {
      return new TargetWriter(target);
    }

    public static implicit operator TargetWriter(Stream target)
    {
      return new TargetWriter(target);
    }

    public static implicit operator TargetWriter(Uri target)
    {
      return new TargetWriter(target);
    }

    public static implicit operator TargetWriter(string targetFile)
    {
      return new TargetWriter(targetFile);
    }

    public static implicit operator TargetWriter(FileInfo targetFile)
    {
      return new TargetWriter(targetFile);
    }
  }
}

