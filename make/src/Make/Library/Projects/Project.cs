﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Make.Library.Subversion;

//
// ESTUDO EM ANDAMENTO
//
// Project irá substituir os vários algoritmos de captura dos
// parâmetros do projeto do PackDm e do Subversion, facilitando
// acrescentar posteriormente informações do GIT e fo NuGet.
//

namespace Make.Library.Projects
{
  class Project
  {
    public ProjectHeader Header { get; set; }
  }
}
