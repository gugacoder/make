using System;
using System.Linq;
using PackDm.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PackDm.IO;
using PackDm.Helpers;

namespace PackDm.Algorithms
{
  public class DistributionResolver
  {
    public string[] ResolveDistribution(FileSystem fileSystem, Pack pack)
    {
      var distFolder = fileSystem.DistFolder;
      var filePaths = EnumerateFiles(distFolder, pack).Distinct();
      var packFiles = ConverFileIntoPackFile(distFolder, filePaths);
      var distFiles = ExceptReservedPackFiles(fileSystem, packFiles);
      return distFiles.ToArray();
    }

    private IEnumerable<string> EnumerateFiles(DirectoryInfo distFolder, Pack pack)
    {
      var browser = new FileBrowser();
      foreach (var dist in pack.Dist)
      {
        var files = browser.EnumerateFiles(distFolder, dist);
        foreach (var item in files)
        {
          yield return item;
        }
      }
    }

    private IEnumerable<string> ConverFileIntoPackFile(DirectoryInfo distFolder, IEnumerable<string> filePaths)
    {
      var prefix = distFolder.FullName;
      var startIndex = prefix.Length + 1;
      return
        from filePath in filePaths
        let relativeFile = filePath.Substring(startIndex)
        let packFile = relativeFile.Replace('\\', '/')
        select packFile;
    }

    private IEnumerable<string> ExceptReservedPackFiles(FileSystem fileSystem, IEnumerable<string> packFiles)
    {
      var reservedPackFiles = new []
      {
        Settings.DefaultConfFile,
        Settings.DefaultPackFile,
        fileSystem.ConfFile.Name,
        fileSystem.PackFile.Name,
        fileSystem.DistFile.Name
      };
      var exception = packFiles.Except(reservedPackFiles);
      return exception;
    }

  }
}

