using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Library.Projects
{
  class ProjectHeader
  {
    public string Group { get; set; }
    public string Artifact { get; set; }
    public VersionInfo Version { get; set; }
  }
}
