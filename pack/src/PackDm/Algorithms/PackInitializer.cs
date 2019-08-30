using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using PackDm.Algorithms;
using PackDm.Model;
using PackDm.SchemaModel;

namespace PackDm.Algorithms
{
  public class PackInitializer
  {
    public FileInfo[] InitializePackProject(Options options, FileSystem fileSystem, Settings settings, Func<Artifact> artifactGetter)
    {
      var files = UpackTemplateFiles(options, fileSystem, settings, artifactGetter);
      return files.ToArray();
    }

    private IEnumerable<FileInfo> UpackTemplateFiles(Options options, FileSystem fileSystem, Settings settings, Func<Artifact> artifactGetter)
    {
      FileInfo confFile = fileSystem.ConfFile;
      FileInfo packFile = fileSystem.PackFile;

      SchemaFile confSchema = LoadSchema(Settings.DefaultConfFile);
      SchemaFile packSchema = LoadSchema(Settings.DefaultPackFile);

      if (!confFile.Exists)
      {
        var port = settings.Port.ToString();
        var folder = settings.RepositoryFolder;

        var proxy = (settings.Proxy != null) ? settings.Proxy.AbsoluteUri : null;
        var proxyHttps = (settings.ProxyHttps != null) ? settings.ProxyHttps.AbsoluteUri : null;

        var packFileName = MakeRelative(fileSystem.PackFile.FullName, Environment.CurrentDirectory);
        var distFileName = MakeRelative(fileSystem.DistFile.FullName, fileSystem.DistFolder.FullName);
        var distFolderName = MakeRelative(fileSystem.DistFolder.FullName, Environment.CurrentDirectory);
        var depsFolderName = MakeRelative(fileSystem.DepsFolder.FullName, Environment.CurrentDirectory);

        var booleanOptions = new List<string>(1);
        if (fileSystem.FlatFolder)
        {
          booleanOptions.Add(Conf.OptionsKey_FlatFolder);
        }

        var uri = settings.RepositoryUris.FirstOrDefault();
        var source = (uri != null) ? uri.AbsoluteUri : null;

        confSchema.SetValue("source", source);
        confSchema.SetValue("proxy", proxy);
        confSchema.SetValue("proxy-https", proxyHttps);
        confSchema.SetValue("port", port);
        confSchema.SetValue("folder", folder);
        confSchema.SetValue("pack-file", packFileName);
        confSchema.SetValue("dist-file", distFileName);
        confSchema.SetValue("dist-folder", distFolderName);
        confSchema.SetValue("deps-folder", depsFolderName);
        confSchema.SetValues("options", booleanOptions);

        // comentando valores default a menos que customizados na linha de comando
        //
        confSchema.CommentProperty("source");
        confSchema.CommentProperty("proxy");
        confSchema.CommentProperty("proxy-https");

        var commentPort = !options.PortOn;
        var commentFolder = !options.FolderOn;
        var commentPackFile = !(options.PackFileOn || options.PackPrefixOn);
        var commentDistFile = !(options.DistFileOn || options.PackPrefixOn);
        var commentDistFolder = !options.DistFolderOn;
        var commentDepsFolder = !options.DepsFolderOn;
        var commendFlatFolder = booleanOptions.Count == 0;

        if (commentPort) confSchema.CommentProperty("port");
        if (commentFolder) confSchema.CommentProperty("folder");
        if (commentPackFile) confSchema.CommentProperty("pack-file");
        if (commentDistFile) confSchema.CommentProperty("dist-file");
        if (commentDistFolder) confSchema.CommentProperty("dist-folder");
        if (commentDepsFolder) confSchema.CommentProperty("deps-folder");
        if (commendFlatFolder) confSchema.CommentProperty("options");

        confSchema.Save(confFile);
        yield return confFile;
      }

      if (!packFile.Exists)
      {
        var artifact = artifactGetter.Invoke();
        var id = artifact.Id;
        packSchema.SetValue("pack", id);
        packSchema.Save(packFile);
        yield return packFile;
      }
    }

    private SchemaFile LoadSchema(string template)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var input = assembly.GetManifestResourceStream(template);
      using (var source = new SourceReader(input))
      {
        var file = SchemaFile.Parse(source);
        return file;
      }
    }

    private string MakeRelative(string path, string referenceFolder)
    {
      if (!referenceFolder.EndsWith(Path.DirectorySeparatorChar.ToString()))
        referenceFolder += Path.DirectorySeparatorChar;

      var uri = new Uri(path);
      var current = new Uri(referenceFolder);

      var relativeUri = current.MakeRelativeUri(uri);
      var relativePath = relativeUri.ToString();

      relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);

      return relativePath;
    }

    private void UnpackFile(string template, FileInfo targetFile)
    {
      var assembly = Assembly.GetExecutingAssembly();
      using (var input = assembly.GetManifestResourceStream(template))
      {
        using (var output = new FileStream(targetFile.FullName, FileMode.Create))
        {
          input.CopyTo(output);
          output.Flush();
        }
      }
    }

    private void ReplaceInFile(FileInfo file, params string[] substitutions)
    {
      ReplaceInFile(file,
        line =>
        {
          for (int i = 0; i < substitutions.Length; i += 2)
          {
            var key = substitutions[i];
            var value = substitutions[i + 1];
            line = line.Replace(key, value);
          }
          return line;
        }
      );
    }

    private void ReplaceAndUncommentInFile(FileInfo file, params object[] substitutions)
    {
      ReplaceInFile(file,
        line =>
        {
          for (int i = 0; i < substitutions.Length; i += 3)
          {
            string key = (string)substitutions[i];
            string value = (string)substitutions[i + 1];
            bool uncomment = (bool)substitutions[i + 2];

            if (!line.Contains(key))
              continue;

            line = line.Replace(key, value);
            if (uncomment)
            {
              line = line.Trim();
              while (line.StartsWith("#"))
              {
                line = line.Substring(1).Trim();
              }
            }

          }
          return line;
        }
      );   
    }

    private void ReplaceInFile(FileInfo file, Func<string, string> replaceInLine)
    {
      var tempfile = Path.GetTempFileName();
      try
      {

        using (var input = file.OpenRead())
        {
          var reader = new StreamReader(input);
          using (var output = File.OpenWrite(tempfile))
          {
            var writer = new StreamWriter(output);

            string line = null;
            while ((line = reader.ReadLine()) != null)
            {
              var replacement = replaceInLine.Invoke(line);
              writer.WriteLine(replacement);
            }

            writer.Flush();
            output.Flush();
          }
        }

        file.Delete();
        File.Move(tempfile, file.FullName);

      }
      catch
      {
        if (File.Exists(tempfile))
        {
          File.Delete(tempfile);
        }
      }
    }

  }
}

