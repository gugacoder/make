using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using Make.Library.Subversion;
using Make.Library.SemVer;
using Make.Library.PackDm;
using System.Text.RegularExpressions;
using Make.Helpers;
using System.Diagnostics;

namespace Make.PlugIns.Subversion
{
  class TagCommand : ICommand<TagCommandOptions>
  {
    public bool Exec(TagCommandOptions options)
    {
      Toolchain.RequirePackDm();
      Toolchain.RequireSubversion();

      if (options.MakeLatest.On)
        return CreateLatestTag(options);
      else
        return CreateTag(options);
    }

    public bool CreateLatestTag(TagCommandOptions options)
    {
      bool ok;

      var user = options.User.Text ?? Svn.DefaultUser;
      var pass = options.Pass.Text ?? Svn.DefaultPass;

      var remote = new SvnRemote(user, pass);
      var workcopy = new SvnWorkcopy(".", user, pass);

      var uri = workcopy.GetInfo(Properties.Url);
      
      var tagNames = remote.GetTagNames(uri, "HEAD");

      if (options.PreRelease.On)
      {
        var regex = new Regex($@"^\d+.\d+.\d+-{options.PreRelease.Text}\d*$");
        tagNames = tagNames.Where(x => regex.IsMatch(x)).ToArray();
      }
      else
      {
        var regex = new Regex(@"^\d+.\d+.\d+$");
        tagNames = tagNames.Where(x => regex.IsMatch(x)).ToArray();
      }

      var maxTag = SemVer.GetMaxVersion(tagNames);
      if (maxTag == null)
        return Prompt.PrintCannotContinue("Não foram encontradas tags válidas na pasta: /tags");

      var tagsUri = remote.MakeSpecialFolderUri(uri, $"tags/{maxTag}");
      var tagsLatest = remote.MakeSpecialFolderUri(uri, $"tags/latest");

      if (options.PreRelease.On)
      {
        tagsLatest = $"{tagsLatest}-{options.PreRelease.Text}";
      }

      if (remote.Exists(tagsLatest, "HEAD"))
      {
        ok = remote.Remove(tagsLatest, "Descartando a revisão \"latest\" para ser recriada.");
        if (!ok) return Prompt.PrintCannotContinue();
      }

      ok = remote.Copy(tagsUri, tagsLatest, "HEAD", $"Criando a revisão \"latest\" com base na revisão {maxTag}");
      if (!ok) return Prompt.PrintCannotContinue();

      return true;
    }

    public bool CreateTag(TagCommandOptions options)
    {
      bool ok;

      var svn = new Svn();
      svn.User = options.User.Text ?? Svn.DefaultUser;
      svn.Pass = options.Pass.Text ?? Svn.DefaultPass;
      ok = svn.Fill();
      if (!ok) return Prompt.PrintCannotContinue();
      
      var pack = new Library.PackDm.PackDm();
      pack.PackConf = options.PackConf.Text;
      pack.PackInfo = options.PackInfo.Text;
      ok = pack.Fill();
      if (!ok) return Prompt.PrintCannotContinue();

      //
      // Validando
      //
      if (svn.IsTag)
      {
        return Prompt.PrintCannotContinue(
            "Não é possível criar uma tag a partir de outra tag diretamente.\n"
          + "O comando deve ser executado a partir de uma pasta trunk ou branch.");
      }

      if (!svn.IsTrunk && !svn.IsBranch)
      {
        return Prompt.PrintCannotContinue(
            "O comando deve ser executado a partir de uma pasta trunk ou branch.");
      }

      if (svn.HasChanges && !options.Force.On)
      {
        return Prompt.PrintCannotContinue(
            "Existem alterações pendentes:\n"
          + "---------- SVN STATUS ----------\n"
          + $"{svn.Changes}\n"
          + "--------------------------------\n"
          + "Resolva as pendências do subversion ou use o argumento --force.");
      }

      //
      // Coletando parametros
      //
      var revision = svn.Revision;

      var tagVersion = pack.Version;
      var newVersion = SemVer.IncreaseMinor(tagVersion);

      var relPath = svn.Url.Strip("/(trunk|branches).*");
      var curPath = svn.Url;
      var tagPath = curPath.Replace(relPath, $"/tags/{tagVersion}");

      if (options.PreRelease.On)
      {
        int rev = 0;
        string version;
        string path;
        do
        {
          rev++;
          version = $"{tagVersion}-{options.PreRelease.Text}{rev}";
          path = $"{tagPath}-{options.PreRelease.Text}{rev}";
        } while (svn.Exists(path));
        tagVersion = version;
        tagPath = path;
      }

      //
      // Criando a tag
      //
      ok = svn.Info(pack.PackInfo); // Apenas imprime informacao de revisao no console
      if (!ok) return Prompt.PrintCannotContinue();

      ok = svn.Copy(curPath, tagPath, $"Fechada a tag {tagVersion} a partir da revisão {revision} de {relPath}.");
      if (!ok) return Prompt.PrintCannotContinue();

      ok = EditRevisionInfo(options, tagPath, tagVersion);
      if (!ok) return Prompt.PrintCannotContinue();

      //
      // Incrementando a versao do trunk, se aplicavel
      //
      if (!options.PreRelease.On)
      {
        pack.Version = newVersion;
        pack.Save();
        if (!ok) return Prompt.PrintCannotContinue();

        ok = svn.Commit(pack.PackInfo, $"Versão de {relPath} incrementada para: {newVersion}");
        if (!ok) return Prompt.PrintCannotContinue();
      }

      return true;
    }

    /// <summary>
    /// Editando os arquivos pack.info e REVISION.txt de uma tag recém criada.
    /// </summary>
    private bool EditRevisionInfo(TagCommandOptions options, string tagPath, string tagVersion)
    {
      var curDir = Directory.GetCurrentDirectory();
      var tmpDir = PathEx.CreateTempFolder();
      try
      {
        bool ok;

        Directory.SetCurrentDirectory(tmpDir);

        var svn = new Svn();
        svn.User = options.User.Text ?? Svn.DefaultUser;
        svn.Pass = options.Pass.Text ?? Svn.DefaultPass;

        ok = svn.Checkout(tagPath, depth: "empty");
        if (!ok) return Prompt.PrintCannotContinue();

        //ok = svn.Update("pack.info pack.conf REVISION.txt");
        ok = svn.Update("REVISION.txt");
        if (!ok) return Prompt.PrintCannotContinue();

        //// editando o pack.info
        //{
        //  var pack = new Library.PackDm.PackDm();
        //  pack.PackConf = "pack.conf";
        //  pack.PackInfo = "pack.info";

        //  ok = pack.Fill();
        //  if (!ok) return Prompt.PrintCannotContinue();

        //  pack.Version = tagVersion;

        //  ok = pack.Save();
        //  if (!ok) return Prompt.PrintCannotContinue();
        //}

        // editando o REVISION.txt
        {
          File.WriteAllText("REVISION.txt", tagVersion);

          // garantindo o versionmanto do arquivo caso ainda nao exista
          svn.Add("REVISION.txt");
        }

        //ok = svn.Commit("pack.info pack.conf REVISION.txt", "Atualizando informação de versão da tag recém criada.");
        ok = svn.Commit("REVISION.txt", "Atualizando informação de versão da tag recém criada.");
        if (!ok) return Prompt.PrintCannotContinue();

        return true;
      }
      catch (Exception ex)
      {
        Prompt.PrintFault(ex);
        return false;
      }
      finally
      {
        Directory.SetCurrentDirectory(curDir);
        try
        {
          PathEx.DeleteFolder(tmpDir);
        }
        catch
        {
          // Nada a fazer.
        }
      }
    }
  }
}
