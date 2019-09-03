using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace PackDm.Helpers
{
  public class FileBrowser
  {
    public IEnumerable<string> EnumerateFiles(DirectoryInfo folder, string pattern)
    {
      List<string> paths = new List<string>();
      paths.Add(folder.FullName);

      var tokens = new Queue<string>(pattern.Split('/'));
      while (tokens.Count > 0)
      {
        var token = tokens.Dequeue();
        if (token == "**")
        {

          var stack = new Stack<string>(paths);
          paths.Clear();
          while (stack.Count > 0)
          {
            var path = stack.Pop();
            if (Directory.Exists(path))
            {
              foreach (var subdir in Directory.EnumerateDirectories(path))
              {
                stack.Push(subdir);
              }

              if (!paths.Contains(path))
                paths.Add(path);

              var content = Directory.EnumerateFiles(path).Except(paths);
              paths.AddRange(content);
            }
          }

        }
        else
        {

          var queue = new Queue<string>(paths);
          paths.Clear();
          while (queue.Count > 0)
          {
            var path = queue.Dequeue();
            if (Directory.Exists(path))
            {
              var content =
                Directory.EnumerateDirectories(path, token)
                  .Union(Directory.EnumerateFiles(path, token))
                  .Except(paths);

              paths.AddRange(content);
            }
          }

        }

      } // while (tokens.Count > 0)

      var files = 
        paths
          .Distinct()
          .Where(p => File.Exists(p))
          .OrderBy(p => p);
      return files;
    }

  }
}

