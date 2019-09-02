using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using PackDm.SchemaModel;

namespace PackDm.Model
{
  public class Conf
  {
    // Configurações do cliente
    public const string SourceUriKey = "source";

    // Configurações do serviço
    public const string PortKey = "port";
    public const string FolderKey = "folder";

    // Configurações gerais
    public const string ProxyKey = "proxy";
    public const string ProxyHttpsKey = "proxy-https";

    // Configurações avançadas
    public const string PackFileKey = "pack-file";
    public const string DistFileKey = "dist-file";
    public const string DistFolderKey = "dist-folder";
    public const string DepsFolderKey = "deps-folder";

    // Registro de configurações boolianas
    public const string OptionsKey = "options";
    public const string OptionsKey_FlatFolder = "flat-folder";

    private readonly Dictionary<string, IEnumerable<string>> entries;

    public Conf()
    {
      this.entries = new Dictionary<string, IEnumerable<string>>();
      this.SetValues(SourceUriKey, Settings.DefaultSourceUri);
    }

    public Schema Schema
    {
      get;
      set;
    }

    public Dictionary<string, IEnumerable<string>> Entries
    {
      get { return entries; }
    }

    public string GetValue(string key)
    {
      var values = GetValues(key);
      return values.LastOrDefault();
    }

    public void SetValue(string key, string value)
    {
      entries[key] = (value != null) ? new [] { value } : Enumerable.Empty<string>();
    }

    public IEnumerable<string> GetValues(string key)
    {
      return entries.ContainsKey(key) ? entries[key] : Enumerable.Empty<string>();
    }

    public void SetValues(string key, IEnumerable<string> values)
    {
      entries[key] = values.ToArray();
    }

    public void SetValues(string key, string value, params string[] values)
    {
      entries[key] = new [] { value }.Union(values).ToArray();
    }
  }
}

