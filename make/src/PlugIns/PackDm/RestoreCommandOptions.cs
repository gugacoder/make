using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Posix;

namespace Make.PlugIns.PackDm
{
  [HelpTitle("Baixa as ferramentas de compilação e as dependências do PackDm e do NuGet.")]
  [HelpUsage("restore [ opcoes ... ]")]
  class RestoreCommandOptions
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
