using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Make.Library.Shell;

namespace Make.Library.Subversion
{
  class SvnWorkcopy
  {
    private readonly string directory;
    private readonly string opts;

    private string[] reservedFilenames = { "do.exe", "do.pdb", "do.config" };

    public SvnWorkcopy(string directory, string user, string pass)
    {
      this.directory = directory;
      this.opts = string.Format(
        " --username {0} --password {1} " + Svn.NoInteractiveOptions
        , user, pass
      );
    }

    #region Operações locais

    public bool Exists
    {
      get { return Directory.Exists(directory); }
    }

    public string GetInfo(string property)
    {
      // property é qualquer propriedade aceita pelo comando "svn info".
      // A classe "Properties.cs" tem constantes para cada uma.

      if (!Directory.Exists(directory))
        return null;

      using (directory.ChDir())
      {
        string output;
        string error;

        var cmd = $"svn info";
        var ok = (cmd + opts).Run(out output, out error);
        if (!ok)
          return null;

        var searchCriteria = property + ": ";

        var value = (
          from line in output.Split('\n', '\r')
          where line.StartsWithIgnoreCase(searchCriteria)
          select line.Substring(searchCriteria.Length)
        ).FirstOrDefault();

        return value;
      }
    }

    public void PurgeWorkcopy()
    {
      if (Directory.Exists(directory))
      {
        DeleteDirectory(directory);
      }
    }

    public bool ShowInfo()
    {
      if (!Directory.Exists(directory))
        return false;

      using (directory.ChDir())
      {
        var cmd = "svn info";
        var ok = (cmd + opts).Run();
        return ok;
      }
    }

    private void DeleteDirectory(string directory)
    {
      var files = Directory.GetFiles(directory);
      var dirs = Directory.GetDirectories(directory);

      foreach (string file in files)
      {
        var filename = Path.GetFileName(file);
        if (reservedFilenames.Contains(filename))
          continue;

        File.SetAttributes(file, FileAttributes.Normal);
        File.Delete(file);
      }

      foreach (string dir in dirs)
      {
        DeleteDirectory(dir);
      }

      if (directory != Environment.CurrentDirectory)
      {
        var hasContent =
          Directory.EnumerateFiles(directory).Any()
          || Directory.EnumerateDirectories(directory).Any();

        if (!hasContent)
        {
          Directory.Delete(directory, false);
        }
      }
    }

    public bool Cleanup()
    {
      if (!Directory.Exists(directory))
        return false;

      using (directory.ChDir())
      {
        var cmd = "svn cleanup";
        var ok = (cmd + opts).Run();
        return ok;
      }
    }

    public bool Revert()
    {
      using (directory.ChDir())
      {
        var cmd = "svn revert . --recursive";
        var ok = (cmd + opts).Run();
        return ok;
      }
    }

    public bool Update(string revision)
    {
      using (directory.ChDir())
      {
        var cmd = string.Format("svn update --revision {0} --parents --force --accept theirs-full", revision);
        var ok = (cmd + opts).Run();
        return ok;
      }
    }

    public bool Checkout(string uri, string revision)
    {
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);

      using (directory.ChDir())
      {
        var cmd = string.Format("svn checkout {0} . --revision {1} --force", uri, revision);
        var ok = (cmd + opts).Run();
        return ok;
      }
    }

    #endregion
  }
}
