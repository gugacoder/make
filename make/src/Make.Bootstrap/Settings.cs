using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Bootstrap
{
  static class Settings
  {
#if NETCOREAPP  // Linux
    public const string ToolName = "make";
    public const string BootstrapName = "make-bootstrap";
    public const string Platform = "linux-x64";
#else           // Windows
    public const string ToolName = "make.exe";
    public const string BootstrapName = "make-bootstrap.exe";
    public const string Platform = "win-x64";
#endif
    public const string BaseUri = "http://keepcoding.net/make/";
  }
}