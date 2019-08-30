using System;
using System.Linq;
using PackDm.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PackDm.IO;
using PackDm.Algorithms;
using System.Text.RegularExpressions;

namespace PackDm.Actions
{
  public class UpgradeAction : IAction
  {
    public void Proceed(Context context)
    {
      var pack = context.Pack;
      var options = context.Options;
      var settings = context.Settings;
      var fileSystem = context.FileSystem;

      IEnumerable<Artifact> dependencies = pack.Deps;
      if (options.FilterOn)
      {
        dependencies = 
          DependencyResolver.FilterArtifacts(dependencies, options.FilterValue);
      }

      var upgrader = new PackUpgrader();

      Exception lastException = null;
      var success = false;

      foreach (var uri in settings.RepositoryUris)
      {
        try
        {
          upgrader.UpgradePackProject(uri, fileSystem.PackFile, dependencies);
          success = true;
          break;
        }
        catch (Exception ex)
        {
          lastException = ex;
        }
      }

      if (!success)
        throw new Exception(lastException.Message, lastException);
    }
  }
}

