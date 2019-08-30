using System;
using PackDm.Model;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using PackDm.Algorithms;
using PackDm.Handlers;
using PackDm.IO;

namespace PackDm.Actions
{
  public class AuthorizeAction : IAction
  {
    public void Proceed(Context context)
    {
      CollectAndSaveCredentials(context);
    }

    private void CollectAndSaveCredentials(Context context)
    {
      var options = context.Options;
      var settings = context.Settings;
      var credentials = context.Credentials;

      var credential = CollectCredential(options);
      
      foreach (var uri in settings.RepositoryUris)
      {
        credentials.SetCredential(uri, credential);
      }

      var file = settings.CredentialsFile;
      CredentialsHandler.Save(credentials, file);
    }

    private Credential CollectCredential(Options options)
    {
      var login = new Credential();
      var input = new InputCollector(options);

      if (options.UserOn)
      {
        login.User = options.UserValue;
      }
      else if (!options.NonInteractiveOn)
      {
        login.User = input.CollectData("Nome de usuário");
      }

      if (options.PassOn)
      {
        login.Pass = options.PassValue;
      }
      else if (!options.NonInteractiveOn)
      {
        login.Pass = input.CollectSecret("Senha para " + (login.User ?? "login"));
      }

      return login;
    }
  }
}

