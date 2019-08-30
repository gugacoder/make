using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using PackDm.Model;

namespace PackDm.Handlers
{
  public static class IndexHandler
  {
    public static Index Load(SourceReader source)
    {
      using (source)
      {
        var reader = source.GetReader();
        var index = new Index();

        Artifact artifact = null;
        
        string line = null;
        while ((line = reader.ReadLine()) != null)
        {
          line = line.Trim();
          
          if (String.IsNullOrEmpty(line))
            continue;
          if (line.StartsWith("#"))
            continue;

          var isDependency = line.StartsWith("+");
          if (isDependency)
          {
            Artifact dependency = line.Split('+').Last().Trim();
            index.RegisterConstraint(artifact, dependency);
          }
          else
          {
            artifact = line;
            index.Add(artifact);
          }
        }

        return index;
      }
    }
  }
}

