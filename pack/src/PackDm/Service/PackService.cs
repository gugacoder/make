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
using System.Text;
using System.Text.RegularExpressions;

namespace PackDm.Service
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  public class PackService : IPackService
  {
    private readonly ResourceHandler resources;
    private readonly string eTag;

    public PackService(string repositoryPath)
      : this(new DirectoryInfo(repositoryPath))
    {
    }

    #region Services...

    public PackService(DirectoryInfo repository)
    {
      this.eTag = Guid.NewGuid().ToString();
      this.resources = new ResourceHandler(repository);
    }

    public Stream Status()
    {
      try
      {
        return Result(HttpStatusCode.OK);
      }
      catch (Exception ex)
      {
        return ResultFault(ex, "Não foi possível determinar o status de execução.");
      }
    }

    public Stream UpdateIndex()
    {
      try
      {
        resources.UpdateIndex();
        return ResultRelocation("./");
      }
      catch (Exception ex)
      {
        return ResultFault(ex, "Falhou a tentativa de reindexar o repositório.");
      }
    }

    public Stream Download(string resource)
    {
      try
      {
        var wcf = OperationContext.Current;
        var web = WebOperationContext.Current;
        var req = web.IncomingRequest;
        var res = web.OutgoingResponse;

        var info = resources.GetResourceInfo(resource);

        if (info.IsCacheable)
        {
          var clientETag = req.Headers["If-None-Match"];
          if (clientETag == eTag)
            return Result(HttpStatusCode.NotModified);

          res.Headers["ETag"] = eTag;
        }

        if (info.IsRedirect)
          return ResultRelocation(info.Location);

        if (info.IsNotFound)
          return Result(HttpStatusCode.NotFound);

        var download = resources.OpenResource(info);

        res.ContentType = download.MimeType;
        if (download.Length > 0)
        {
          res.ContentLength = download.Length;
        }

        wcf.OperationCompleted += (o, e) => download.Stream.Dispose();

        return download.Stream;
      }
      catch (Exception ex)
      {
        return ResultFault(ex, "Falhou a tentativa de preparar o download do recurso: " + resource);
      }
    }

    public Stream Upload(string resource, Stream content)
    {
      try
      {
        var web = WebOperationContext.Current;
        var res = web.OutgoingResponse;

        var info = resources.GetResourceInfo(resource);

        if (info.IsRedirect)
          return ResultRelocation(info.Location);

        if (info.IsFolder)
          return Result(HttpStatusCode.BadRequest, "Resource Is Not a File");

        if (!(info.IsNotFound || info.IsFile))
          return Result(HttpStatusCode.BadRequest);

        resources.SaveResource(info, content);

        return Result(HttpStatusCode.OK);
      }
      catch (Exception ex)
      {
        return ResultFault(ex, "Falhou a tentativa de arquivar o recurso: " + resource);
      }
    }

    public Stream Delete(string resource)
    {
      try
      {
        var web = WebOperationContext.Current;
        var res = web.OutgoingResponse;

        var info = resources.GetResourceInfo(resource);

        if (info.IsRedirect)
          return ResultRelocation(info.Location);

        if (info.IsNotFound)
          return Result(HttpStatusCode.NotFound);

        if (!(info.IsFolder || info.IsFile))
          return Result(HttpStatusCode.BadRequest);

        resources.DeleteResource(info);

        return Result(HttpStatusCode.OK);
      }
      catch (Exception ex)
      {
        return ResultFault(ex, "Falhou a tentativa de excluir o recurso: " + resource);
      }
    }

    #endregion

    #region Métodos de auxílio...

    private Stream Result(HttpStatusCode statusCode)
    {
      return Result(statusCode, null, null);
    }

    private Stream ResultRelocation(string relocation)
    {
      return Result(HttpStatusCode.Found, null, relocation);
    }

    private Stream Result(HttpStatusCode statusCode, string description)
    {
      return Result(statusCode, description, null);
    }

    private Stream Result(HttpStatusCode statusCode, string description, string relocation)
    {
      if (description == null)
      {
        if (statusCode == HttpStatusCode.OK)
        {
          description = "OK";
        }
        else
        {
          var text = statusCode.ToString();
          description = Regex.Replace(text, "([A-Z])", " $1");
        }
      }

      var web = WebOperationContext.Current;
      var res = web.OutgoingResponse;

      res.StatusCode = statusCode;
      res.StatusDescription = description;
      res.Location = relocation;

      var xml =
        new XDocument(
          new XElement("PackDm",
            new XElement("Status", (int)statusCode),
            new XElement("Description", description)
          )
        );

      if (relocation != null)
        xml.Root.Add(new XElement("Location", relocation));

      return CreateResult(xml);
    }

    private Stream ResultFault(Exception ex, string message)
    {
      return ResultFault(ex, HttpStatusCode.InternalServerError, message);
    }

    private Stream ResultFault(Exception ex, HttpStatusCode statusCode, string description)
    {
      if (description == null)
      {
        if (statusCode == HttpStatusCode.OK)
        {
          description = "OK";
        }
        else
        {
          var text = statusCode.ToString();
          description = Regex.Replace(text, "([A-Z])", " $1");
        }
      }

      if (ex != null)
        DumpException(description, ex);

      var wcf = OperationContext.Current;
      var web = WebOperationContext.Current;
      var res = web.OutgoingResponse;

      res.StatusCode = statusCode;

      var xml =
        new XDocument(
          new XElement("PackDm",
            new XElement("Status", (int)statusCode),
            new XElement("Description", description),
            new XElement("StackTrace", new XCData(ex.GetStackTrace()))
          )
        );

      return CreateResult(xml);
    }

    private Stream CreateResult(XDocument xml)
    {
      var wcf = OperationContext.Current;
      var web = WebOperationContext.Current;
      var res = web.OutgoingResponse;

      var memory = new MemoryStream();
      xml.Save(memory);
      memory.Position = 0;

      res.ContentType = "application/xml; charset=utf-8";
      res.ContentLength = memory.Length;

      wcf.OperationCompleted += (o, e) => memory.Dispose();

      return memory;
    }

    private string GetStackTrace(string message, Exception ex)
    {
      var builder = new StringBuilder();
      builder.Append("fault: ");
      builder.AppendLine(message);
      while (ex != null)
      {
        builder.Append("cause: ");
        builder.AppendLine(ex.Message);
        builder.Append("type: ");
        builder.AppendLine(ex.GetType().FullName);
        builder.Append("stack: ");
        builder.AppendLine(ex.StackTrace);
        ex = ex.InnerException;
      }
      return builder.ToString();
    }

    private void DumpException(string message, Exception ex)
    {
      Console.Write("[fault][");
      Console.Write(DateTime.Now);
      Console.Write("]");
      Console.WriteLine(message);
      while (ex != null)
      {
        Console.Write("cause: ");
        Console.WriteLine(ex.Message);
        Console.Write("type: ");
        Console.WriteLine(ex.GetType().FullName);
        Console.Write("stack: ");
        Console.WriteLine(ex.StackTrace);
        ex = ex.InnerException;
      }
    }

    #endregion

  }
}

