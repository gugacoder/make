using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Do.Library.Shell;

namespace Do.Library.Subversion
{
  class SvnRemote
  {
    private string opts;

    public SvnRemote(string user, string pass)
    {
      this.opts = string.Format(
        " --username {0} --password {1} " + Svn.NoInteractiveOptions
        , user, pass
      );
    }

    public bool Exists(string uri, string revision)
    {
      var cmd = string.Format("svn info {0} --revision {1}", uri, revision);
      var ok = (cmd + opts).Run();
      return ok;
    }

    public bool Remove(string uri, string message)
    {
      var cmd = string.Format("svn rm {0} -m \"{1}\"", uri, message);
      var ok = (cmd + opts).Run();
      return ok;
    }

    public bool Copy(string sourceUri, string targetUri, string revision, string message)
    {
      var cmd = string.Format("svn cp {0} {1} -r {2} -m \"{3}\"", sourceUri, targetUri, revision, message);
      var ok = (cmd + opts).Run();
      return ok;
    }

    public string[] GetTagNames(string uri, string revision)
    {
      var exists = HasSpecialFolder(uri, revision, "tags");
      if (!exists)
        return new string[0];

      var radical = uri.Split("trunk", "tags", "branches").First();
      if (!radical.EndsWith("/"))
        radical += "/";

      var tagsUri = radical + "tags";
      var tags = ListContent(tagsUri, revision);
      return tags;
    }

    public string[] ListContent(string uri, string revision)
    {
      string output;
      string error;

      var cmd = string.Format("svn ls {0} --revision {1}", uri, revision);
      var ok = (cmd + opts).Run(out output, out error);
      if (!ok)
        throw new Exception(error);

      var lines = (
        from token in output.Split('\n', '\r')
        where !string.IsNullOrWhiteSpace(token)
        select token.EndsWith("/") ? token.Substring(0, token.Length - 1) : token
      ).ToArray();

      return lines;
    }

    public string MakeSpecialFolderUri(string uri, string specialFolderName)
    {
      // specialFolderName pode ser:
      // - trunk
      // - tags
      // - branches

      var radical = uri.Split("trunk", "tags", "branches").First();
      if (!radical.EndsWith("/"))
        radical += "/";

      var specialUri = radical + specialFolderName;
      return specialUri;
    }

    public bool HasSpecialFolder(string uri, string revision, string specialFolderName)
    {
      // specialFolderName pode ser:
      // - trunk
      // - tags
      // - branches

      var radical = uri.Split("trunk", "tags", "branches").First();
      if (!radical.EndsWith("/"))
        radical += "/";

      var tags = ListContent(radical, revision);
      return tags.Contains(specialFolderName);
    }

  }
}
