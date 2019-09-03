using System;
using System.Linq;
using PackDm.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PackDm.IO;
using PackDm.Helpers;
using System.Text.RegularExpressions;

namespace PackDm.Algorithms
{
  public class DependencyResolver
  {
    public static IEnumerable<Artifact> FilterArtifacts(IEnumerable<Artifact> dependencies, IList<string> filters)
    {
      var patterns = filters.Select(f => "^" + f.Replace("*", ".*") + "$");
      var regexes = patterns.Select(p => new Regex(p));

      var filteredDependencies = (
        from artifact in dependencies
        where regexes.Any(regex => regex.IsMatch(artifact))
        select artifact
      ).ToArray();

      return filteredDependencies;
    }

    public IEnumerable<Artifact> ResolveArtifacts(Model.Index index, IEnumerable<Artifact> dependencies)
    {
      Console.WriteLine("[index][tree]Solving dependencies...");

      var map = new Dictionary<string, IEnumerable<Artifact>>();
      foreach (var dependency in dependencies)
      {
        var key = dependency.Group + "/" + dependency.Name;
        var matches = index.FindMatches(dependency);

        if (map.ContainsKey(key))
        {
          var artifactIds = map[key].Select(x => x.Id);
          var matchesIds = matches.Select(x => x.Id);
          var intersectedIds = artifactIds.Intersect(matchesIds);
          var intersectedArtifacts = intersectedIds.Select(x => new Artifact { Id = x });
          matches = intersectedArtifacts;

          if (!matches.Any())
            throw new PackDmException("Não foi possível resolver uma versão compatível para o artefato porque existem referências desencontradas: " + dependency);
        }
        else
        {
          if (!matches.Any())
            throw new PackDmException("Não foi possível resolver uma versão compatível para o artefato: " + dependency);
        }

        map[key] = matches;
      }

      var artifacts = map.Select(m => m.Value.LastOrDefault());
      return artifacts;
    }

    private IEnumerable<FileInfo> EnumerateFiles(string pattern)
    {
      List<string> paths = new List<string>();
      paths.Add("Dist");

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
              paths.Add(path);
              foreach (var subdir in Directory.EnumerateDirectories(path))
              {
                stack.Push(subdir);
              }
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
              paths.AddRange(Directory.EnumerateDirectories(path, token));
              paths.AddRange(Directory.EnumerateFiles(path, token));
            }
          }

        }

      } // while (tokens.Count > 0)

      var files = paths.Where(p => File.Exists(p)).Select(p => new FileInfo(p));
      return files;
    }

  }
}

