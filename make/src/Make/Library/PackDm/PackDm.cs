using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Make.Library.Posix;
using Make.Library.Shell;

namespace Make.Library.PackDm
{
  class PackDm
  {
    private string packInfo;

    public string PackConf { get; set; }

    public string PackInfo
    {
      get { return packInfo ?? "pack.info"; }
      set { packInfo = value; }
    }

    public string Content { get; private set; }

    public string Id
    {
      get
      {
        var lines = Content.Split('\n', '\r');
        var id =
          lines
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .SkipWhile(x => x != "pack")
            .Skip(1)
            .Select(x => x.Trim())
            .First();
        return id;
      }
      set
      {
        var name = Id;
        Content = Content.Replace(name, value);
      }
    }

    public string Group
    {
      get { return GetPart(0); }
      set { SetPart(0, value); }
    }

    public string Artifact
    {
      get { return GetPart(1); }
      set { SetPart(1, value); }
    }

    public string Version
    {
      get { return GetPart(2); }
      set { SetPart(2, value); }
    }

    public bool Fill()
    {
      try
      {
        Content = PackInfo.Read();
        return true;
      }
      catch (Exception ex)
      {
        Prompt.PrintFault(ex);
        return false;
      }
    }

    public bool Save()
    {
      try
      {
        Content.Save(PackInfo);
        return true;
      }
      catch (Exception ex)
      {
        Prompt.PrintFault(ex);
        return false;
      }
    }

    public bool Init()
    {
      var opts = "";
      if (PackConf != null) opts += $" --conf-file {PackConf.Quote()}";
      if (PackInfo != null) opts += $" --pack-file {PackInfo.Quote()}";

      var ok = $"pack init {opts}".Run();
      if (!ok) return false;

      return Fill();
    }

    public bool Pack(string versionMask = null)
    {
      var opts = "";

      if (versionMask != null) opts += "--set-version " + versionMask;
      if (PackConf != null) opts += $" --conf-file {PackConf.Quote()}";
      if (PackInfo != null) opts += $" --pack-file {PackInfo.Quote()}";

      var ok = $"pack pack {opts}".Run();
      return ok;
    }

    public bool Deploy()
    {
      var opts = "";
      if (PackConf != null) opts += $" --conf-file {PackConf.Quote()}";
      if (PackInfo != null) opts += $" --pack-file {PackInfo.Quote()}";

      var ok = $"pack deploy {opts}".Run();
      return ok;
    }

    public bool Install()
    {
      var opts = "";
      if (PackConf != null) opts += $" --conf-file {PackConf.Quote()}";
      if (PackInfo != null) opts += $" --pack-file {PackInfo.Quote()}";

      var ok = $"pack install {opts}".Run();
      return ok;
    }

    public bool Upgrade()
    {
      var opts = "";
      if (PackConf != null) opts += $" --conf-file {PackConf.Quote()}";
      if (PackInfo != null) opts += $" --pack-file {PackInfo.Quote()}";

      var ok = $"pack upgrade {opts}".Run();
      return ok;
    }

    private string GetPart(int index)
    {
      var id = Id;
      var version = id.Split('/')[index];
      return version;
    }

    private void SetPart(int index, string value)
    {
      var id = Id;
      var tokens = id.Split('/');
      tokens[index] = value;
      var newId = string.Join("/", tokens);
      Content = Content.Replace(id, newId);
    }
  }
}
