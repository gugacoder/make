using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Make.Helpers;
using Make.Library.Subversion;

//
// ESTUDO EM ANDAMENTO
//
// Project irá substituir os vários algoritmos de captura dos
// parâmetros do projeto do PackDm e do Subversion, facilitando
// acrescentar posteriormente informações do GIT e fo NuGet.
//

namespace Make.Library.Projects
{
  class ProjectLoader
  {
    public string PackConf { get; set; }
    public string PackInfo { get; set; }
    public string SvnUser { get; set; }
    public string SvnPass { get; set; }

    public Ret<Project> LoadProject(string folder = ".")
    {
      try
      {
        bool ok;

        var svn = new Svn();
        svn.User = SvnUser ?? Svn.DefaultUser;
        svn.Pass = SvnPass ?? Svn.DefaultPass;
        ok = svn.Fill();
        if (!ok) Ret.Fail();

        var pack = new PackDm.PackDm();
        pack.PackConf = PackConf;
        pack.PackInfo = PackInfo;
        ok = pack.Fill();
        if (!ok) Ret.Fail();

        var project = new Project
        {
          Header = LoadProjectHeader(svn, pack)
        };
        return project;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    public Ret<ProjectHeader> LoadProjectHeader(string folder = ".")
    {
      try
      {
        bool ok;

        var svn = new Svn();
        svn.User = SvnUser ?? Svn.DefaultUser;
        svn.Pass = SvnPass ?? Svn.DefaultPass;
        ok = svn.Fill();
        if (!ok) Ret.Fail();

        var pack = new PackDm.PackDm();
        pack.PackConf = PackConf;
        pack.PackInfo = PackInfo;
        ok = pack.Fill();
        if (!ok) Ret.Fail();

        var header = LoadProjectHeader(svn, pack);
        return header;
      }
      catch (Exception ex)
      {
        return ex;
      }
    }

    private static ProjectHeader LoadProjectHeader(Svn svn, PackDm.PackDm pack)
    {
      var header = new ProjectHeader
      {
        Group = pack.Group,
        Artifact = pack.Artifact,
        Version = pack.Version
      };

      if (header.Version.PreRelease == null)
      {
        header.Version.PreRelease = InferPreRelease();
      }

      header.Version.Revision = int.TryParse(svn.Revision, out int n) ? n : 0;

      return header;
    }

    private static string InferPreRelease()
    {
      // tentando obter a revisao do REVISION.txt
      var revisionInfo = RevisionFileLoader.GetRevisionInfo();
      if (!string.IsNullOrWhiteSpace(revisionInfo.PreRelease))
      {
        return revisionInfo.PreRelease;
      }

      // tentando obter a revisao do svn
      var svn = new Svn();
      var ok = svn.Fill();
      if (ok)
      {
        var url = svn.Url;

        Match match;

        // Quando a URL contém informacao de versao na forma: X.X.X-prereleaseX
        match = Regex.Match(url, @"\d+(?:\.\d+)?(?:\.\d+)?(?:-([a-zA-Z\d]+))");
        if (match.Success)
        {
          return match.Groups[1].Value;
        }

        // Quando a URL contém trunk ou branches
        match = Regex.Match(url, @"(trunk|branch)");
        if (match.Success)
        {
          return match.Groups[1].Value;
        }
      }

      return null;
    }
  }
}
