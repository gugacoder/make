using System;
using System.Net;
using PackDm.Model;

namespace PackDm.IO
{
  public class WebClientFactory
  {
    public static readonly WebClientFactory Current = new WebClientFactory();

    private WebClientFactory()
    {
    }

    public Credentials Credentials
    {
      get;
      set; 
    }

    public Uri Proxy
    {
      get;
      set;
    }

    public Uri ProxyHttps
    {
      get;
      set;
    }

    public WebClient CreateWebClient(Uri uri)
    {
      var web = new CustomWebClient();

      if (Credentials != null)
      {
        var credential = Credentials.GetCredential(uri);
        if (credential != null)
        {
          web.Credentials = new NetworkCredential(credential.User, credential.Pass);
        }
      }

      if (uri.Scheme == "http")
      {
        if (Proxy != null)
        {
          web.Proxy = new WebProxy(Proxy);
        }
      }
      else if (uri.Scheme == "https")
      {
        if (ProxyHttps != null)
        {
          web.Proxy = new WebProxy(ProxyHttps);
        }
      }

      return web;
    }

  }
}

