using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using PackDm.Model;
using PackDm.SchemaModel;

namespace PackDm.Handlers
{
  public static class PackHandler
  {
    public static Pack Load(SourceReader source)
    {
      var schema = Schema.Parse(source);
      var deps = schema.GetValues("deps");
      var dist = schema.GetValues("dist");

      var pack = new Pack();
      pack.Id = schema.GetValue("pack");
      pack.Deps = deps.Select(x => new Artifact { Id = x }).ToList();
      pack.Dist = dist.ToList();
      pack.Schema = schema;

      return pack;
    }

    public static void Save(Pack pack, TargetWriter target)
    {
      var schema = pack.Schema ?? new Schema();

      schema.SetValue("pack", pack.Id);
      schema.SetValues("deps", pack.Deps.Select(x => x.Id));
      schema.SetValues("dist", pack.Dist);

      schema.Save(target);
    }
  }
}

