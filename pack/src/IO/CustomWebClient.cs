using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PackDm.IO
{
  public class CustomWebClient : WebClient
  {
    /// <summary>
    /// Tempo de espera por resposta do servidor remoto em segundos.
    /// </summary>
    /// <value>Tempo de espera em segundos.</value>
    public int Timeout { get; set; }

    public CustomWebClient()
    {
    }

    protected override WebRequest GetWebRequest(Uri uri)
    {
      var request = base.GetWebRequest(uri);
      var httpRequest = request as HttpWebRequest;
      if (httpRequest != null)
      {
        var milliseconds = (Timeout > 0) ? (Timeout * 1000) : int.MaxValue;
        httpRequest.Timeout = milliseconds;
        httpRequest.ReadWriteTimeout = milliseconds;
      }
      return request;
    }
  }
}
