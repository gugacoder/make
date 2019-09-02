using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Make.Library.Posix;
using Make.Library.Shell;

namespace Make.Library.Subversion
{
  class Svn
  {
    private readonly NameValueCollection properties = new NameValueCollection();
    private string changes;

    public const string DefaultUser = "subversion";
    public const string DefaultPass = "#qwer0987";

    public const string NoInteractiveOptions = "--no-auth-cache --non-interactive --trust-server-cert";

    public string User { get; internal set; }
    public string Pass { get; internal set; }

    public string Url => properties?["URL"];
    public string Revision => properties?["Revision"];

    public bool IsTag => Url?.Contains("/tags") ?? false;
    public bool IsBranch => Url?.Contains("/branches") ?? false;

    public bool IsTrunk => Url?.Contains("/trunk") ?? false;

    public bool HasChanges => !string.IsNullOrWhiteSpace(changes);
    public string Changes => changes;

    internal bool Fill()
    {
      try
      {
        bool ok;

        string info;

        properties.Clear();

        var opts = "";
        if (User != null) opts += $" --username {User.Quote()}";
        if (Pass != null) opts += $" --password {Pass.Quote()}";

        ok = $"svn info {opts} {NoInteractiveOptions}".Run(out info);
        if (!ok) return false;

        ok = $"svn status {opts} {NoInteractiveOptions}".Run(out changes);
        if (!ok) return false;

        var entries =
          from line in info.Split('\n', '\r')
          where line.Contains(':')
          let charIndex = line.IndexOf(':')
          let key = line.Substring(0, charIndex).Trim()
          let value = line.Substring(charIndex + 1).Trim()
          select new { key, value };

        entries.ForEach(entry => properties[entry.key] = entry.value);

        return true;
      }
      catch (Exception ex)
      {
        Prompt.PrintFault(ex);
        return false;
      }
    }

    internal bool Exists(string path)
    {
      var opts = "";
      if (User != null) opts += $" --username {User.Quote()}";
      if (Pass != null) opts += $" --password {Pass.Quote()}";

      var ok = $"svn info {path} {opts} {NoInteractiveOptions}".Run(out string output);
      return ok;
    }

    internal bool Update(string files = null)
    {
      var opts = "";
      if (User != null) opts += $" --username {User.Quote()}";
      if (Pass != null) opts += $" --password {Pass.Quote()}";

      var ok = $"svn update {files} {opts} {NoInteractiveOptions}".Run();
      if (!ok) return false;

      return Fill();
    }

    internal bool Info(string path)
    {
      var ok = $"svn info {path}".Run();
      return ok;
    }

    internal bool Copy(string sourcePath, string targetPath, string message)
    {
      var opts = "";
      if (User != null) opts += $" --username {User.Quote()}";
      if (Pass != null) opts += $" --password {Pass.Quote()}";

      var ok = $"svn copy {sourcePath} {targetPath} {opts} {NoInteractiveOptions} -m {message.Quote()}".Run();
      return ok;
    }

    internal bool Checkout(string tagPath, string depth = "infinity")
    {
      var opts = "";
      if (User != null) opts += $" --username {User.Quote()}";
      if (Pass != null) opts += $" --password {Pass.Quote()}";

      var ok = $"svn checkout {tagPath} . --depth {depth} {opts} {NoInteractiveOptions}".Run();
      if (!ok) return false;

      return Fill();
    }

    internal bool Commit(string files, string message)
    {
      var opts = "";
      if (User != null) opts += $" --username {User.Quote()}";
      if (Pass != null) opts += $" --password {Pass.Quote()}";

      var ok = $"svn commit {files} {opts} {NoInteractiveOptions} -m {message.Quote()}".Run();
      if (!ok) return false;

      return Update();
    }

    internal bool Add(string files)
    {
      var ok = $"svn add {files}".Run();
      return ok;
    }

    internal bool Ignore(params string[] patterns)
    {
      bool ok;
      string ignoreList = null;

      ok = $"svn pget svn:ignore .".Run(out ignoreList);
      if (!ok) return false;

      var tokens = (
        from line in ignoreList.Split('\n', '\r')
        where !string.IsNullOrWhiteSpace(line)
        select line
      ).Distinct().ToList();

      var changes = patterns.Except(tokens);
      if (!changes.Any())
        return true;

      tokens.AddRange(changes);

      ignoreList = string.Join(Environment.NewLine, tokens);

      ok = $"svn pset svn:ignore {ignoreList.Quote()} .".Run();
      if (!ok) return false;

      return true;
    }
  }
}
