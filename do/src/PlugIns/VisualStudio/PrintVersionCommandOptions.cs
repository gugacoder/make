using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Do.Library.Posix;

namespace Do.PlugIns.VisualStudio
{
  [HelpTitle("Compila uma solução do VisualStudio.")]
  [HelpUsage("print-version [ opcoes ... ]")]
  class PrintVersionCommandOptions
  {
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
