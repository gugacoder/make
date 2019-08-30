using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Do.Library.Posix;

namespace Do.PlugIns.Core
{
  [HelpTitle("Gera um GUID e imprime.")]
  [HelpUsage("guid")]
  class GuidCommandOptions
  {
    [Opt('f')]
    [OptArg("FORMATO")]
    [Help(
      "Formato de impressão do GUID gerado:",
      "   N   32 dígitos:",
      "       00000000000000000000000000000000",
      "   D   32 dígitos separados por hifens:",
      "       00000000-0000-0000-0000-000000000000",
      "   B   32 dígitos separados por hifens, entre chaves:",
      "       {{00000000-0000-0000-0000-000000000000}}",
      "   P   32 dígitos separados por hifens, entre parênteses:",
      "       (00000000-0000-0000-0000-000000000000)",
      "Se omitido, o formato padrão usado será: B"
    )]
    public OptArg Format { get; set; }

    [Opt('c')]
    [OptArg("QUANTIDADE")]
    [Help(
      "Quantidade de códigos GUID a gerar.",
      "Será impresso um GUID por linha.",
      "Se omitido apenas um GUID é gerado."
    )]
    public OptArg<int> Count { get; set; }
  }
}
