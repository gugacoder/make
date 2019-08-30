using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Do.Library.Posix;

namespace Do.PlugIns.Msi
{
  [HelpTitle("Comandos de interação com pacotes MSI do Windows Installer.")]
  [HelpUsage("msi COMANDO PACOTE_MSI [ OPÇÕES ]")]
  class MsiCommandOptions
  {
    [Operand("COMANDO", "PACOTE_MSI")]
    public OptArgArray Operands { get; set; }

    [Opt('o')]
    [Help(
      "Caminho do arquivo destino gerado.",
      "Se omitido o nome será definido pelo algoritmo."
    )]
    public OptArg OutputFile { get; set; }

    [Opt('d')]
    [Help(
      "Pasta destino para geração do arquivo."
    )]
    public OptArg OutputDir { get; set; }
    
    [Opt('s', Category = "Opções de Clonagem")]
    [Help(
      "Sufixo de versão.",
      "Geralmente o nome de uma versão seguido de uma revisão do nome da versão.",
      "Como em: beta1, beta2, beta3, etc.",
      "Quando um pre-release é usado a porta de serviço, se aplicável, é recalculada.",
      "A menos que o parâmetro --service-port seja indicado.",
      "Algumas portas mais comuns produzidas são;",
      "-   alfa  9082",
      "-   beta  9090",
      "-   trunk 9242"
    )]
    public OptArg PreRelease { get; set; }

    [Opt('i', Category = "Opções de Clonagem")]
    [Help(
      "Infere um nome de pre-release a partir da leitura do arquivo REVISION.txt.",
      "É esperado que o arquivo REVISION.txt tenha uma única declaração",
      "de versão na forma:",
      "   X.X.X-releaseX_rX",
      "Sendo",
      "   X         - Um número qualquer.",
      "   X.X.X     - Obrigatório. O número de versão do aplicativo.",
      "   releaseX  - Opcional. Um nome de pre-release seguido de uma revisão de pre-release.",
      "               Como em: beta1, beta2, beta3, etc.",
      "   rX        - Opcional. O número de revisão do código fonte no repositório.",
      "Quando um pre-release é usado a porta de serviço, se aplicável, é recalculada.",
      "A menos que o parâmetro --service-port seja indicado.",
      "Algumas portas mais comuns produzidas são;",
      "-   alfa  9082",
      "-   beta  9090",
      "-   trunk 9242"
    )]
    public Opt InferPreRelease { get; set; }

    [Opt('n', Category = "Opções de Clonagem")]
    [Help(
      "Define um nome de produto para o novo setup gerado.",
      "Se omitido o nome será definido pelo algoritmo."
    )]
    public OptArg ProductName { get; set; }

    [Opt('t', Category = "Opções de Clonagem")]
    [Help(
      "Título do serviço de fundo do Windows instalado pelo pacote MSI.",
      "Se omitido o título será definido pelo algoritmo."
    )]
    public OptArg ProductTitle { get; set; }

    [Opt('p', Category = "Opções de Clonagem")]
    [Help(
      "Define a porta de lançamento do serviço instalado pelo setup clonado.",
      "Se omitido a porta será definida pelo algoritmo."
    )]
    public OptArg<int> ServicePort { get; set; }

    [HelpSection("Comandos", Order = SectionOrder.AfterUsage)]
    public IEnumerable<string> PrintCommands()
    {
      yield return "make-uninstall";
      yield return "        Constrói um desinstalador para o pacote MSI.";
      yield return "        Opções:";
      yield return "          --output-file";
      yield return "          --output-dir";
      yield return "          --service-name";
      yield return " ";
      yield return "update";
      yield return "        Modifica as proprieades de um arquivo MSI.";
      yield return "        Se --pre-release ou --infer-pre-release for indicado as propriedades";
      yield return "        ProductName e ServiceName serão acrescidas desse pre-release.";
      yield return "        Opções:";
      yield return "          --pre-release";
      yield return "          --infer-pre-release";
      yield return "          --product-name";
      yield return "          --product-title";
      yield return "          --service-port";
      yield return " ";
      yield return "clone";
      yield return "        Clona um arquivo MSI modificando suas propriedades.";
      yield return "        Se --pre-release ou --infer-pre-release for indicado as propriedades";
      yield return "        ProductName e ServiceName serão acrescidas desse pre-release.";
      yield return "        Opções:";
      yield return "          --output-file";
      yield return "          --output-dir";
      yield return "          --pre-release";
      yield return "          --infer-pre-release";
      yield return "          --product-name";
      yield return "          --product-title";
      yield return "          --service-port";
      yield return "show-info";
      yield return "        Imprime as propriedades do pacote MSI.";
      yield return "list-files";
      yield return "        Lista os arquivos contigos no pacote MSI.";
    }
  }
}
