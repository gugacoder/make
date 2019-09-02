using System;
using System.Linq;
using System.Collections.Generic;

namespace PackDm.SchemaModel
{
  public class Schema
  {
    private readonly SchemaFile file;

    public Schema()
    {
      this.file = new SchemaFile();
    }

    public Schema(SchemaFile file)
    {
      this.file = file;
    }

    public SchemaFile File
    {
      get{ return file; }
    }

    public IEnumerable<string> Keys
    {
      get
      {
        return
          from i in file.GetItems()
          where i.IsProperty
          select i.Text;
      }
    }

    public void AddProperty(string key)
    {
      var exists = (file.GetProperty(key) != null);
      if (!exists)
      {
        file.AddProperty(key);
      }
    }

    public IEnumerable<string> GetValues(string key)
    {
      var values = file.GetValues(key);
      return values.Select(v => v.Text);
    }

    public void SetValues(string key, IEnumerable<string> values)
    {
      AddProperty(key);
      file.ClearProperty(key);
      foreach (var value in values)
      {
        file.AddValue(key, value);
      }
    }

    public void SetValues(string key, string value, params string[] values)
    {
      AddProperty(key);
      file.ClearProperty(key);
      file.AddValue(key, value);
      foreach (var item in values)
      {
        file.AddValue(key, item);
      }
    }

    public string GetValue(string key)
    {
      var values = GetValues(key);
      return values.LastOrDefault();
    }

    public void SetValue(string key, string value)
    {
      AddProperty(key);
      file.ClearProperty(key);
      file.AddValue(key, value);
    }

    #region Manipulação...

    public static Schema Parse(SourceReader source)
    {
      var file = SchemaFile.Parse(source);
      return new Schema(file);
    }

    public void Save(TargetWriter target)
    {
      this.file.Save(target);
    }

    #endregion

  }
}

