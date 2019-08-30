using System;
using System.Collections.Generic;
using PackDm.Model;

namespace PackDm.Handlers
{
  public static class CredentialsHandler
  {
    public static void Save(Credentials credentials, TargetWriter target)
    {
      using (target)
      {
        var writer = target.GetWriter();

        foreach (var entry in credentials)
        {
          var uri = entry.Key;
          var token = entry.Value;

          var encryptedData = token.GetEncriptedData();

          var line = uri + ' ' + encryptedData;
          writer.WriteLine(line);
        }

        writer.Flush();
      }
    }

    public static Credentials Load(SourceReader source)
    {
      using (source)
      {
        var reader = source.GetReader();
        var tokens = new Credentials();

        string line = null;

        while ((line = reader.ReadLine()) != null)
        {
          try
          {

            if (String.IsNullOrWhiteSpace(line))
              continue;
            if (line.TrimStart().StartsWith("#"))
              continue;

            var parts = line.Split(' ');
            if (parts.Length != 2)
            {
              Console.WriteLine("Token de autorização mal formado: " + line);
              continue;
            }

            var uri = parts[0];
            var encryptedData = parts[1];

            var token = new Credential();
            token.SetEncriptedData(encryptedData);

            tokens.Add(uri, token);
          }
          catch (Exception ex)
          {
            throw new PackDmException(
              "Falhou a tentativa de interpretar o arquivo de configuração.\nPróximo de:\n  " + line,
              ex);
          }
        }

        return tokens;
      }
    }

    public static string CreateSpace(string uri)
    {
      return CreateSpace(new Uri(uri));
    }

    public static string CreateSpace(Uri uri)
    {
      return uri.Scheme + "://" + uri.Host + ":" + uri.Port;
    }

  }
}

