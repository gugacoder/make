using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.Library.Subversion
{
  /// <summary>
  /// Coleção das propriedades disponíveis para consulta pelo comando "svn info".
  /// </summary>
  static class Properties
  {
    public const string Kind = "Node Kind";
    public const string Url = "URL";
    public const string RelativeUrl = "Relative URL";
    public const string ReposRootUrl = "Repository Root";
    public const string ReposUuid = "Repository UUID";
    public const string Revision = "Revision";
    public const string LastChangedRevision = "Last Changed Rev";
    public const string LastChangedDate = "Last Changed Date";
    public const string LastChangedAuthor = "Last Changed Author";
    public const string WcRoot = "Working Copy Root Path";
  }
}
