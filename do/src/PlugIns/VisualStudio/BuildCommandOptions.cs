using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Do.Library.Posix;

namespace Do.PlugIns.VisualStudio
{
  [HelpTitle("Compila uma solução do VisualStudio.")]
  [HelpUsage("build solucao [ opcoes ... ]")]
  class BuildCommandOptions
  {
    [Opt('c')]
    [Help(
      "Configuração da solução do VisualStudio construída.",
      "Por padrão: \"Release|Any CPU\""
    )]
    public OptArg Configuration { get; set; }

    [Operand("Solução")]
    public OptArgArray Solutions { get; set; }

    [Opt]
    [Help(
      "Caminho alternativo para o arquivo de configuração do PackDm.",
      "Por padrão o arquivo tem o nome de: pack.conf"
    )]
    public OptArg ConfFile { get; set; }

    [Opt]
    [Help(
      "Caminho alternativo para o arquivo de especificação do artefato.",
      "Por padrão o arquivo tem o nome de: pack.info"
    )]
    public OptArg PackFile { get; set; }
  }
}
