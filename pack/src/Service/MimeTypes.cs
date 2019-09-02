using System;
using System.IO;

namespace PackDm.Service
{
  public static class MimeTypes
  {
    public static string GetMimeType(string file)
    {
      return GetMimeType(new FileInfo(file));
    }

    public static string GetMimeType(FileInfo file)
    {
      switch (file.Extension)
      {
        case ".md": return "text/plain; charset=UTF-8";
        case ".txt": return "text/plain; charset=UTF-8";
        case ".info": return "text/plain; charset=UTF-8";
        case ".index": return "text/plain; charset=UTF-8";
        case ".html": return "text/html; charset=UTF-8";
        case ".css": return "text/css; charset=UTF-8";
        case ".js": return "text/javascript; charset=UTF-8";
        case ".xml": return "application/xml; charset=UTF-8";
        case ".json": return "application/json; charset=UTF-8";
        case ".xslt": return "application/xslt+xml; charset=UTF-8";
        default: return "application/octet-stream";
      }
    }
  }
}

