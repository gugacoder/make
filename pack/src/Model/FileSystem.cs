using System;
using System.IO;

namespace PackDm.Model
{
  public class FileSystem
  {
    public FileInfo ConfFile
    {
      get { return confFile ?? (confFile = new FileInfo(Settings.DefaultConfFile)); }
      set { confFile = value; }
    }
    private FileInfo confFile;

    public FileInfo PackFile
    {
      get { return packFile ?? (packFile = new FileInfo(Settings.DefaultPackFile)); }
      set { packFile = value; }
    }
    private FileInfo packFile;

    public FileInfo DistFile
    {
      get
      {
        if (distFile != null)
          return distFile;
        
        var folder = DistFolder.FullName;
        var file = new FileInfo(Path.Combine(folder, Settings.DefaultPackFile));
        return file;
      }
      set { distFile = value; }
    }
    private FileInfo distFile;

    public DirectoryInfo DistFolder
    {
      get { return distFolder ?? (distFolder = new DirectoryInfo(Settings.DefaultDistFolder)); }
      set { distFolder = value; }
    }
    private DirectoryInfo distFolder;

    public DirectoryInfo DepsFolder
    {
      get { return depsFolder ?? (depsFolder = new DirectoryInfo(Settings.DefaultDepsFolder)); }
      set { depsFolder = value; }
    }
    private DirectoryInfo depsFolder;

    public bool FlatFolder
    {
      get;
      set;
    }
  }
}

