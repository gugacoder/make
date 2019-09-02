using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Make.Library.Posix;

namespace Make.PlugIns.Subversion
{
  [HelpTitle("Fecha uma versão com a criação de uma tag.")]
  [HelpUsage("tag [ opcoes ... ]")]
  class TagCommandOptions
  {
    [Opt('f')]
    [Help(
      "Força a criação de uma tag."
    )]
    public Opt Force { get; set; }

    [Opt('l')]
    [Help(
      "Em vez de fechar uma nova versão apenas refaz a tag latest com",
      "a versão mais recente encontrada em tags."
    )]
    public Opt MakeLatest { get; set; }

    [Opt('s')]
    [Help(
      "Nomeia uma tag com um sufixo.",
      "A tag criada recebe o sufixo e um número incremental de revisão.",
      "Quando um sufixo é usado o número de revisão declarado no arquivo",
      "pack.info não é incrementada.",
      "O sufixo é ideal para criação de tags ALFA e BETA.",
      "Por exemplo, considere os comandos:",
      "  tag --pre-release beta",
      "  tag --pre-release beta --make-latest",
      "Seriam criadas as seguintes tags, respectivamente:",
      "  /tags/x.y.z-beta1",
      "  /tags/latest-beta1",
      "Caso a tag já existisse o número seria incrementado produzindo:",
      "  /tags/x.y.z-beta2",
      "  /tags/latest-beta2"
    )]
    public OptArg PreRelease { get; set; }

    [Opt('u')]
    [Help(
      "Nome do usuário do subversion.",
      "Se omitido o usuário padrão chamado \"subversion\" é utilizado."
    )]
    public OptArg User { get; set; }

    [Opt('p')]
    [Help(
      "Senha do usuário do subversion."
    )]
    public OptArg Pass { get; set; }

    [Opt]
    [Help(
      "Caminho alternativo para o arquivo de configuração do PackDm.",
      "Por padrão: pack.conf"
    )]
    public OptArg PackConf { get; set; }

    [Opt]
    [Help(
      "Caminho alternativo para o arquivo de especificação do artefato.",
      "Por padrão: pack.info"
    )]
    public OptArg PackInfo { get; set; }
  }
}
