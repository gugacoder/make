using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackDm.Bootstrap
{
  static class Settings
  {
#if NETCOREAPP  // Linux
    public const string ToolName = "pack";
    public const string BootstrapName = "pack-bootstrap";
    public const string Platform = "linux-x64";
#else           // Windows
    public const string ToolName = "pack.exe";
    public const string BootstrapName = "pack-bootstrap.exe";
    public const string Platform = "win-x64";
#endif
    public const string BaseUri = "http://keepcoding.net/pack/";
  }
}