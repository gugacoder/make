using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Package;

namespace Do.Library.Msi
{
  class MsiBundle : IDisposable
  {
    private InstallPackage package;

    public struct Entry
    {
      public InstallPath Info { get; set; }
      public string File { get; set; }
      public string WorkCopyFile { get; set; }
    }

    public MsiBundle(string filepath, DatabaseOpenMode mode)
    {
      this.package = new InstallPackage(filepath, mode);
    }

    public Entry[] FindFiles(string filename)
    {
      var files = (
        from file in package.Files
        where file.Value.TargetName == filename
        select new Entry
        {
          Info = file.Value,
          File = file.Key,
          // Montando o caminho onde ExtractFiles extraiu o arquivo
          WorkCopyFile = 
            Path.GetFullPath(Path.Combine(package.WorkingDirectory, file.Value.TargetName))
        }
      ).ToArray();
      return files;
    }

    public void ExtractFiles(IEnumerable<Entry> entries)
    {
      package.ExtractFiles(entries.Select(x => x.File).ToArray());
    }

    public void CommitFiles(IEnumerable<Entry> entries)
    {
      package.UpdateFiles(entries.Select(x => x.File).ToArray());
    }

    public void Dispose()
    {
      package.Dispose();
    }
  }
}
