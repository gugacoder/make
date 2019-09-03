using System;
using System.IO;
using System.Net;
using PackDm.IO;

namespace PackDm
{
  public class SourceReader : IDisposable
  {
    private event Action Disposed;
    private readonly TextReader reader;

    public SourceReader(FileInfo sourceFile)
      : this(sourceFile.FullName)
    {

    }

    public SourceReader(string sourceFile)
    {
      if (File.Exists(sourceFile))
      {
        var stream = new FileStream(sourceFile, FileMode.Open);
        this.Disposed += () => stream.Dispose();

        this.reader = new StreamReader(stream);
      }
      else
      {
        this.reader = new StreamReader(Stream.Null);
      }
    }

    public SourceReader(Uri source)
    {
      Console.WriteLine("[uri]" + source);
      var web = WebClientFactory.Current.CreateWebClient(source);

      var stream = web.OpenRead(source.CreateNoCachedUriVersion());
      this.Disposed += () => stream.Dispose();

      this.reader = new StreamReader(stream);
    }

    public SourceReader(Stream source)
    {
      this.reader = new StreamReader(source);
    }

    public SourceReader(TextReader source)
    {
      this.reader = source;
    }

    public TextReader GetReader()
    {
      return reader;
    }

    public void Dispose()
    {
      if (Disposed != null)
      {
        Disposed.Invoke();
      }
    }

    public static implicit operator TextReader(SourceReader source)
    {
      return source.reader;
    }

    public static implicit operator SourceReader(TextReader source)
    {
      return new SourceReader(source);
    }

    public static implicit operator SourceReader(Stream source)
    {
      return new SourceReader(source);
    }

    public static implicit operator SourceReader(Uri source)
    {
      return new SourceReader(source);
    }

    public static implicit operator SourceReader(string sourceFile)
    {
      return new SourceReader(sourceFile);
    }

    public static implicit operator SourceReader(FileInfo sourceFile)
    {
      return new SourceReader(sourceFile);
    }
  }
}

