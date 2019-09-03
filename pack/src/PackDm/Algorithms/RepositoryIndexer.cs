using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PackDm.Model;
using System.Threading;
using PackDm.Handlers;

namespace PackDm.Algorithms
{
  public class RepositoryIndexer
  {
    private readonly DirectoryInfo folder;
    private static readonly object synclock = new object();

    public RepositoryIndexer(string folder)
    {
      this.folder = new DirectoryInfo(folder);
    }

    public RepositoryIndexer(DirectoryInfo folder)
    {
      this.folder = folder;
    }

    public void UpdateIndex()
    {
      if (Monitor.TryEnter(synclock))
      {
        try
        {
          if (!folder.Exists)
          {
            folder.Create();
          }

          var indexFile = CreateIndexFile();
          SaveIndexFile(indexFile);
        }
        finally
        {
          Monitor.Exit(synclock);
        }
      }
    }

    private FileInfo CreateIndexFile()
    {
      var indexFile = new FileInfo(Path.GetTempFileName());
      Console.Write("[index]indexing ");
      Console.WriteLine(folder.FullName);

      try
      {
        using (var stream = indexFile.OpenWrite())
        {
          var writer = new StreamWriter(stream);

          writer.WriteLine(string.Format(
            "#\n"
          + "# Índice do repositório.\n"
          + "# {0}\n"
          + "#\n"
          + "# A indentação com " + " refere-se às dependências da biblioteca.\n"
          + "# Por exemplo, se o aplicativo App dependende da biblioteca Lib a entrada de\n"
          + "# índice seria:\n"
          + "#    Grupo/App/1.0.0\n"
          + "#    + Grupo/Lib/1.0.0\n"
          + "#\n"
          , DateTime.Now
          ));

          var stack = new Stack<DirectoryInfo>();
          stack.Push(folder);

          while (stack.Count > 0)
          {
            var directory = stack.Pop();
            foreach (var subdirectory in directory.EnumerateDirectories())
            {
              stack.Push(subdirectory);
            }

            var files = directory.EnumerateFiles();
            var packFile = files.SingleOrDefault(f => f.Name == Settings.DefaultPackFile);
            if (packFile != null)
            {

              try
              {
                var pack = PackHandler.Load(packFile);
                writer.WriteLine(pack.Id);
                foreach (var dep in pack.Deps)
                {
                  writer.Write("+ ");
                  writer.WriteLine(dep);
                }
              }
              catch (Exception ex)
              {
                Program.DumpException("O arquivo de pacote não pode ser lido: " + packFile, ex);
              }
 
            }
          }

          writer.WriteLine();
          writer.Flush();
        }

        return indexFile;
      }
      catch (Exception ex)
      {
        if (indexFile.Exists)
        {
          try
          {
            indexFile.Delete();
          }
          catch
          {
            // nada a fazer...
          }
        }
        throw ex;
      }
    }

    private void SaveIndexFile(FileInfo indexFile)
    {
      var targetFile = new FileInfo(Path.Combine(folder.FullName, "pack.index"));

      if (!targetFile.Directory.Exists)
      {
        targetFile.Directory.Create();
      }
      if (targetFile.Exists)
      {
        targetFile.Delete();
      }

      indexFile.MoveTo(targetFile.FullName);

      Console.WriteLine("[index][saved]" + indexFile.FullName);
    }

  }
}

