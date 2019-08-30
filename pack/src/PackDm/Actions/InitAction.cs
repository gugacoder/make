using System;
using PackDm.Model;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using PackDm.Algorithms;
using PackDm.IO;

namespace PackDm.Actions
{
  public class InitAction : IAction
  {
    public void Proceed(Context context)
    {
      var options = context.Options;
      var settings = context.Settings;
      var fileSystem = context.FileSystem;

      var template = new PackInitializer();
      var files = template.InitializePackProject(options, fileSystem, settings, () => CollectData(context));
      if (files.Length == 0)
      {
        Console.WriteLine("Nada a ser criado.");
      }
      else
      {
        foreach (var file in files)
        {
          Console.Write(file.Name);
          Console.WriteLine(" criado.");
        }
      }
    }

    private Artifact CollectData(Context context)
    {
      var options = context.Options;

      var artifact = new Artifact();
      var input = new InputCollector(options);

      if (options.ArtifactGroupOn)
      {
        artifact.Group = options.ArtifactGroupValue;
      }
      else
      {
        var messageLines = new []
        {
          "Grupo do artefato, geralmente um de:",
          "  KeepCoding",
          "  3rd-party"
        };
        artifact.Group = input.CollectData(messageLines) ?? "KeepCoding";
      }

      if (options.ArtifactNameOn)
      {
        artifact.Name = options.ArtifactNameValue;
      }
      else
      {
        artifact.Name = input.CollectData("Nome do artefato") ?? "Artefato";
      }

      if (options.ArtifactVersionOn)
      {
        artifact.Version = options.ArtifactVersionValue;
      }
      else
      {
        var messageLines = new[]
        {
          "Versão do artefato no formato `major.minor.patch`,",
          "Por exemplo: 1.0.0",
          "O último digito geralmente é omitido para ser definido",
          "no momento da publicação do pacote."
        };

        var version = input.CollectData(messageLines);
        artifact.Version = string.IsNullOrWhiteSpace(version) ? "1.0.0" : version;
      }

      return artifact;
    }

  }
}

