using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PackDm.Model;
using PackDm.Algorithms;

namespace PackDm.Actions
{
  public class IndexAction : IAction
  {
    public void Proceed(Context context)
    {
      var settings = context.Settings;
      var repositoryFolder = new DirectoryInfo(settings.RepositoryFolder);

      var indexer = new RepositoryIndexer(repositoryFolder);
      indexer.UpdateIndex();
    }
  }
}

