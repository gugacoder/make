using System;
using System.ServiceModel;
using PackDm.Model;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace PackDm.Service
{
  public class PackHost : IDisposable
  {
    public PackHost(int port, string folder)
    {
      this.Uri = new Uri("http://localhost:" + port);
      this.Host = CreateHost(this.Uri, folder);

      Host.Opened += (o, e) => IsRunning = true;
      Host.Closed += (o, e) => IsRunning = false;
    }

    private ServiceHost CreateHost(Uri uri, string folder)
    {
      var service = new PackService(folder);
      var host = new WebServiceHost(service, new []{ uri });

      var duasHoras = new TimeSpan(2, 0, 0);

      host.OpenTimeout = duasHoras;
      host.CloseTimeout = duasHoras;

      foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
      {
        var binding = endpoint.Binding;
        binding.OpenTimeout = duasHoras;
        binding.CloseTimeout = duasHoras;
        binding.ReceiveTimeout = duasHoras;
        binding.SendTimeout = duasHoras;

        var webBinding = binding as WebHttpBinding;
        if (webBinding != null)
        {
          webBinding.MaxReceivedMessageSize = long.MaxValue;
          webBinding.UseDefaultWebProxy = true;
          webBinding.TransferMode = TransferMode.StreamedRequest;
        }
      }

      return host;
    }

    public Uri Uri
    {
      get;
      private set;
    }

    public ServiceHost Host
    {
      get;
      private set;
    }

    public bool IsRunning
    {
      get;
      private set;
    }

    public void Open()
    {
      Host.Open();
    }

    public void Close()
    {
      Host.Close();
    }

    public void Dispose()
    {
      ((IDisposable)Host).Dispose();
    }
  }
}

