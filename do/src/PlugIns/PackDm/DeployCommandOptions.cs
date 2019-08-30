using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Do.Library.Posix;

namespace Do.PlugIns.PackDm
{
  [HelpTitle("Publica bibliotecas no repositório do PackDm.")]
  [HelpUsage("deploy [ opcoes ... ]")]
  class DeployCommandOptions
  {
    [Opt]
    [Help(
      "Caminho alternativo para o arquivo de configuração do PackDm.",
      "Por padrão o arquivo tem o nome de: pack.conf"
    )]
    public OptArg PackConf { get; set; }

    [Opt]
    [Help(
      "Caminho alternativo para o arquivo de especificação do artefato.",
      "Por padrão o arquivo tem o nome de: pack.info"
    )]
    public OptArg PackInfo { get; set; }
  }
}
