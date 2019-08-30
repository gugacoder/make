using System;
using System.Collections.Generic;

namespace PackDm.Model
{
  public class Credentials : Dictionary<string, Credential>
  {
    public static string GetSection(Uri uri)
    {
      return uri.Scheme + "://" + uri.Host + ":" + uri.Port;
    }

    public Credential GetCredential(Uri uri)
    {
      var section = Credentials.GetSection(uri);
      return ContainsKey(section) ? this[section] : null;
    }

    public Credential SetCredential(Uri uri, Credential credential)
    {
      var section = Credentials.GetSection(uri);
      return this[section] = credential;
    }

    public void RemoveCredential(Uri uri)
    {
      var section = Credentials.GetSection(uri);
      if (ContainsKey(section))
      {
        Remove(section);
      }
    }

    #region Métodos para URIs em strings

    public static string GetSection(string uri)
    {
      return GetSection(new Uri(uri));
    }

    public Credential GetCredential(string uri)
    {
      return GetCredential(new Uri(uri));
    }

    public Credential SetCredential(string uri, Credential credential)
    {
      return SetCredential(new Uri(uri), credential);
    }

    public void RemoveCredential(string uri)
    {
      RemoveCredential(new Uri(uri));
    }

    #endregion
  }
}

