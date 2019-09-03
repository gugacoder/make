using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using PackDm.Model;
using PackDm.SchemaModel;
using System.Linq;

namespace PackDm.Handlers
{
  public static class ConfHandler
  {
    public static Conf Load(SourceReader source)
    {
      var schema = Schema.Parse(source);

      var conf = new Conf();
      conf.Schema = schema;

      foreach (var key in schema.Keys)
      {
        var values = schema.GetValues(key);
        conf.SetValues(key, values);
      }

      return conf;
    }

    public static void Save(Conf conf, TargetWriter target)
    {
      var schema = conf.Schema ?? new Schema();

      foreach (var entry in conf.Entries)
      {
        var key = entry.Key;
        var values = entry.Value;
        schema.SetValues(key, values);
      }

      schema.Save(target);
    }
  }
}

