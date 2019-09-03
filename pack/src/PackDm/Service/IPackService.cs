using System;
using System.ServiceModel;
using System.IO;
using System.ServiceModel.Web;
using PackDm.Model;
using System.Net;
using System.Xml.Linq;
using System.Xml;
using PackDm.Actions;
using System.Threading.Tasks;
using PackDm.Algorithms;

namespace PackDm.Service
{
  [ServiceContract]
  public interface IPackService
  {
    [OperationContract]
    [WebGet(UriTemplate = "Status")]
    Stream Status();

    [OperationContract]
    [WebGet(UriTemplate = "Reindex")]
    Stream UpdateIndex();

    [OperationContract]
    [WebGet(UriTemplate = "{*resource}")]
    Stream Download(string resource);

    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "{*resource}")]
    Stream Upload(string resource, Stream content);

    [OperationContract]
    [WebInvoke(Method = "DELETE", UriTemplate = "{*resource}")]
    Stream Delete(string resource);
  }
}

