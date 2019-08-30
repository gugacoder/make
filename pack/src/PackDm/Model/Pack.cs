using System;
using System.IO;
using System.Collections.Generic;
using PackDm.SchemaModel;

namespace PackDm.Model
{
  public class Pack : Artifact
  {
    public List<Artifact> Deps
    {
      get { return deps ?? (deps = new List<Artifact>()); }
      set{ deps = value; }
    }
    private List<Artifact> deps;

    public List<string> Dist
    {
      get { return dist ?? (dist = new List<string>()); }
      set{ dist = value; }
    }
    private List<string> dist;

    public Schema Schema
    {
      get;
      set;
    }

    public void NormalizeDistNames()
    {
      for (var i = 0; i < Dist.Count; i++)
      {
        var dist = Dist[i];
        Dist[i] = dist.Replace("\\", "/");
      }
    }
  }
}

