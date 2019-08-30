using System;
using System.Linq;
using PackDm.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PackDm.IO;
using PackDm.Algorithms;
using System.Text.RegularExpressions;
using PackDm.Handlers;

namespace PackDm.Actions
{
  public class SearchAction : IAction
  {
    public void Proceed(Context context)
    {
      var pack = context.Pack;
      var options = context.Options;
      var settings = context.Settings;
      var fileSystem = context.FileSystem;

      Exception lastException = null;

      foreach (var uri in settings.RepositoryUris)
      {
        try
        {
          var indexUri = new Uri(uri, "pack.index");
          var index = IndexHandler.Load(indexUri);

          var artifacts = FilterArtifacts(index, options);
          foreach (var artifact in artifacts)
          {
            Console.WriteLine(artifact.ToString());
          }

          break;
        }
        catch (Exception ex)
        {
          lastException = ex;
        }
      }

      if (lastException != null)
        throw new Exception(lastException.Message, lastException);
    }

    private IEnumerable<Artifact> FilterArtifacts(IEnumerable<Artifact> dependencies, Options options)
    {
      if (!options.FilterOn)
        return dependencies;

      var filters = options.FilterValue;
      var patterns = filters.Select(f => "^" + f.Replace("*", ".*") + "$");
      var regexes = patterns.Select(p => new Regex(p));

      var filteredDependencies = (
        from artifact in dependencies
        where regexes.Any(regex => regex.IsMatch(artifact))
        select artifact
      ).ToArray();

      return filteredDependencies;
    }
  }
}

