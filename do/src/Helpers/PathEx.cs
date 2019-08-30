using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.Helpers
{
  /// <summary>
  /// Funções de extensão do utilitário System.IO.Path
  /// </summary>
  static class PathEx
  {
    public static string CreateTempFile()
    {
      return Path.GetTempFileName();
    }

    public static string CreateTempFolder()
    {
      var filepath = Path.GetTempFileName();
      File.Delete(filepath);
      Directory.CreateDirectory(filepath);
      return filepath;
    }

    public static void DeleteFolder(string folder)
    {
      DeleteFolder(new DirectoryInfo(folder));
    }

    public static void DeleteFolder(DirectoryInfo folder)
    {
      foreach (var subFolder in folder.GetDirectories())
      {
        DeleteFolder(subFolder);
      }

      foreach (var file in folder.GetFiles())
      {
        file.Attributes = FileAttributes.Normal;
        file.Delete();
      }

      folder.Attributes = FileAttributes.Normal;
      folder.Delete();
    }
  }
}
