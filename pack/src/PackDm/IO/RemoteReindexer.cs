using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackDm.IO
{
  public class RemoteReindexer
  {
    public void Reindex(Uri uri)
    {
      var text = uri.ToString();
      if (!text.EndsWith("/")) text += "/";
      text += "Reindex";

      uri = new Uri(text);
      Console.WriteLine("[indexing]" + uri.AbsoluteUri);

      var web = WebClientFactory.Current.CreateWebClient(uri);
      web.OpenRead(uri.CreateNoCachedUriVersion());

      Console.WriteLine("[indexing][ok]" + uri.AbsoluteUri);
    }
  }
}
