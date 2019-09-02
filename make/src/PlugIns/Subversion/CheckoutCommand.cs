using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using Make.Library.Subversion;
using Make.Library.SemVer;

namespace Make.PlugIns.Subversion
{
  class CheckoutCommand : ICommand<CheckoutCommandOptions>
  {
    public bool Exec(CheckoutCommandOptions options)
    {
      Toolchain.RequireSubversion();

      bool ok;

      if (!options.Repository.On)
        return Prompt.PrintInvalidUsage("A URI do repositório Subversion não foi indicada.");

      var revision = options.Revision.On ? options.Revision.Text : "HEAD";
      var user = options.User.On ? options.User.Text : Svn.DefaultUser;
      var pass = options.Pass.On ? options.Pass.Text : Svn.DefaultPass;

      var folder = options.Directory.On ? options.Directory.Text : ".";
      folder = Path.GetFullPath(folder);

      if (!Directory.Exists(folder))
        Directory.CreateDirectory(folder);

      Prompt.PrintInfo("Workcopy: \n" + folder);

      var workcopy = new SvnWorkcopy(folder, user, pass);
      var remote = new SvnRemote(user, pass);

      var tag = GetTagName(options);
      if (tag == "latest")
      {
        var baseUri = options.Repository.Text;
        var tagNames = remote.GetTagNames(baseUri, revision);

        var tagName = SemVer.GetMaxVersion(tagNames);
        if (tagName == null)
        {
          var exists = remote.HasSpecialFolder(baseUri, revision, "trunk");
          tagName = exists ? "trunk" : "self";
        }

        tag = tagName;
      }

      var uri = GetRepositoryUri(options, tag);

      var workcopyUri = workcopy.GetInfo(Properties.Url);
      if (options.ForceCheckout.On || uri != workcopyUri)
      {
        workcopy.PurgeWorkcopy();

        ok = workcopy.Checkout(uri, revision);
        if (!ok) return Prompt.PrintCannotContinue();

        ok = workcopy.Cleanup();
        if (!ok) return Prompt.PrintCannotContinue();
      }
      else
      {
        ok = workcopy.Cleanup();
        if (!ok) return Prompt.PrintCannotContinue();

        ok = workcopy.Revert();
        if (!ok) return Prompt.PrintCannotContinue();

        ok = workcopy.Cleanup();
        if (!ok) return Prompt.PrintCannotContinue();

        ok = workcopy.Update(revision);
        if (!ok) return Prompt.PrintCannotContinue();
      }

      workcopy.ShowInfo();
      Prompt.PrintInfo("Concluído com êxito.");
      return true;
    }

    private static string GetTagName(CheckoutCommandOptions options)
    {
      string tag;

      if (options.Tag.On)
      {
        tag = string.IsNullOrWhiteSpace(options.Tag.Text) ? "latest" : options.Tag.Text;
        tag = tag.Replace("branches", "branch");
      }
      else
      {
        var uri = options.Repository.Text;

        if (uri.Contains("tags"))
        {
          var path = uri.Split("tags").LastOrDefault();
          while (path.StartsWith("/"))
            path = path.Substring(1);

          tag = path;
        }
        else if (uri.Contains("branches"))
        {
          var path = uri.Split("branches").LastOrDefault();
          if (!path.StartsWith("/"))
            path = "/" + path;

          tag = "branch" + path;
        }
        else if (uri.Contains("trunk"))
        {
          tag = "trunk";
        }
        else
        {
          tag = "latest";
        }
      }

      while (tag.StartsWith("/"))
      {
        tag = tag.Substring(1);
      }

      return tag;
    }

    private static string GetRepositoryUri(CheckoutCommandOptions options, string tag)
    {
      var uri = options.Repository.Text;

      //
      // Determinando o radical da URL
      //
      string radical;
      {
        radical = uri.Split("trunk", "tags", "branches").First();
        if (!radical.EndsWith("/"))
          radical += "/";
      }

      //
      // Determinando a URL
      //
      string repositoryUri;
      {
        if (tag == "self")
          repositoryUri = uri;
        else if (tag == "trunk")
          repositoryUri = radical + "trunk";
        else if (tag.StartsWith("branch"))
          repositoryUri = radical + tag.Replace("branch", "branches");
        else
          repositoryUri = radical + "tags/" + tag;
      }

      return repositoryUri;
    }
  }
}
