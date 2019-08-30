using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Do.Library.Posix;

namespace Do.PlugIns.Subversion
{
  [HelpTitle("Realiza o checkout ou update de um repositório subversion.")]
  [HelpUsage("checkout REPO [ -t TAG ] [ -f ] [ -r REV ] [ -d FOLDER ] [ -u USER ] [ -p PASS ]")]
  [HelpSection(
    "Login de usuário",
    "Não é necessário indicar login de usuário.",
    "A ferramenta loga automaticamente o usuário padrão \"subversion\""
  )]
  [HelpSection(
    "Tags",
    "Uma tag pode ser:",
    "  self           Para indicar a própria URL do repositório.",
    "  trunk          Para indicar a pasta trunk.",
    "  N.N.N          Para indicar um número de versao.",
    "  branch/TAL     Para indicar um determinado branch.",
    "Ou as variações:",
    "  latest         Para indicar a versao mais recente.",
    "                 O batch analista a pasta tags e escolhe a versao mais",
    "                 recente encontrada.",
    "  branches/TAL   O mesmo que \"branch/TAL\".",
    " ",
    "Se o parametro--tag for omitido o batch escolhe a tag padrao da seguinte forma:",
    "- Se a URI tiver as palavras trunk, tags ou branches entao sera a propria tag",
    "- Senao, a versao mais recente encontrada na pasta \"/tags\" sera assumida.",
    "- Senao houver versão na pasta \"/tags\" então trunk será assumido.",
    "- Senao houver a pasta \"/trunks\" então a própria pasta será usada.",
    " ",
    "De acordo com a tag indicada o batch reescreve a URI do repositorio.",
    "O batch corta a URI no ponto onde uma das palavras trunk, tags ou branches e",
    "encontrada e completa a URI com o nome da tag.",
    " ",
    "Por exemplo, para:",
    "  http://host.com/svn/meu-projeto/trunk",
    "Se indicado a tag:",
    "  1.2.0",
    "A URI e rescrita como:",
    "  http://host.com/svn/meu-projeto/tags/1.2.0",
    "Ou, se indicado a tag:",
    "  branch/meus/testes",
    "A URI e rescrita como:",
    "  http://host.com/svn/meu-projeto/branches/meus/testes"
  )]
  [HelpSection(
    "Exemplo",
    "{0} checkout http://serverpro/svn/users/sandbox/projeto --tag 1.5.0"
  )]
  [HelpFooter("Copyleft (ɔ) - All rights reversed.")]
  class CheckoutCommandOptions
  {
    [Opt('t')]
    [OptArg("TAG")]
    [Help(
      "Nome de uma tag para baixar.",
      "Um de:",
      "  self         Para indicar a própria URL indicada no comando.",
      "  trunk        Para indicar a pasta /trunk.",
      "  N.N.N        Indicando um número de versão na pasta /tags.",
      "  branch/TAL   Indicando uma subpasta qualquer da pasta /branches"
    )]
    public OptArg Tag { get; set; }

    [Opt('f')]
    [Help(
      "Força uma operação de checkout.",
      "Todo o conteúdo da pasta de trabalho é removido e um \"svn checkout\"",
      "é executado novamente."
    )]
    public Opt ForceCheckout { get; set; }

    [Opt('r')]
    [OptArg("REV")]
    [Help(
      "Número de revisão da cópia de trabalho.",
      "Se omitido, a versão HEAD será usada."
    )]
    public OptArg Revision { get; set; }

    [Opt('d')]
    [OptArg("FOLDER")]
    [Help(
      "Pasta da cópia de trabalho.",
      "Se omitido, o repositório será baixado para a pasta corrente."
    )]
    public OptArg Directory { get; set; }

    [Opt('u')]
    [OptArg("USER")]
    [Help(
      "Nome do usuário do subversion.",
      "Se omitido, o usuário padrão chamado \"subversion\" será usado."
    )]
    public OptArg User { get; set; }

    [Opt('p')]
    [OptArg("PASS")]
    [Help("Senha do usuário do subversion.")]
    public OptArg Pass { get; set; }

    [Operand("REPO")]
    [Help("URI do repositório Subversion.")]
    public OptArg Repository { get; set; }
  }
}
