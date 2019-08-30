using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using PackDm.Service;

namespace PackDm.Model
{
  public partial class Options
  {
    public const string OpcoesDeCliente = "Opções de Cliente";
    public const string OpcoesDeServico = "Opções de Serviço";
    public const string OpcoesGerais = "Opções Gerais";
    public const string OpcoesAvancadas = "Opções Avançadas";

    #region Ações...

    [Argument]
    [Actions(
      "init",
      "install",
      "pack",
      "deploy",
      "search",
      "serve",
      "index",
      "authorize",
      "upgrade"
    )]
    [Description(
      "Ações de cliente:",
      "  init [ --pack-prefix VALOR",
      "         --artifact-group VALOR --artifact-name VALOR --artifact-version VALOR ]",
      "    Inicializa um projeto do PackDm criando o arquivo pack.info e correlatos.",
      "  install [ --filter FILTRO ]",
      "    Baixa as dependências declaradas no arquivo de configuração pack.info.",
      "  pack [ --set-version VALOR ]",
      "    Constrói a solução e prepara a distribuição do pacote na pasta `dist`",
      "  deploy",
      "    Publica o pacote construído na pasta `dist` no servidor de pacotes.",
      "  search FILTRO",
      "    Pesquisa um pacote no repositório.",
      "    O catacter `*' pode ser usado para indicar qualquer texto na posição.",
      "  authorize [ --user VALOR --pass VALOR ]",
      "    Armazena em cache as credenciais de comunicação com o serviço do PackDm.",
      "  upgrade [ --filter FILTRO ]",
      "    Atualiza o pack.info com a versão mais recente das dependências.",
      "Ações de serviço:",
      "  serve [ --folder VALOR --port VALOR ]",
      "    Inicia uma instância do servidor de artefatos.",
      "  index [ --folder VALOR ]",
      "    Indexa um repositório de artefatos produzindo o arquivo `pack.index`."
    )]
    public bool ChainOn { get; set; }
    public string ChainValue { get; set; }

    #endregion

    #region Argumentos do cliente...

    [Argument('v', "set-version", OpcoesDeCliente)]
    [Description(
      "Define a versão do pacote produzido.",
      "VALOR define a versão no formato:",
      "  0.0.0-tag",
      "Os caracteres `x` e `*` podem ser usados como curingas:",
      "  x.x.x-x",
      "Na posição do curinga prevalesce o valor obtido da versão no `pack.info`.",
      "Exemplos:",
      "  Versão     Máscara     Resultado ",
      "  ---------  ----------  ----------",
      "  1.2-alfa   x.x.4       1.2.4     ",
      "  1.2-alfa   x.x.4-x     1.2.4-alfa",
      "  1.2-alfa   x.x.4-beta  1.2.4-beta",
      "  1-alfa     x.x.4       1.0.4     ",
      "  1-alfa     x.x.4-x     1.0.4-alfa",
      "  1-alfa     x.x.4-beta  1.0.4-beta",
      "  2.0.1      x.5         2.5.1     ",
      "  2.5        x.7.0       2.7.0     ",
      "  2.5        3.x.x       3.5.0     "
    )]
    public bool SetVersionOn { get; set; }
    public string SetVersionValue { get; set; }

    [Argument("artifact-group", OpcoesDeCliente)]
    [Description(
      "Nome do grupo do artefato.",
      "Geralmente um de:",
      "- KeepCoding",
      "- 3rd-party"      
    )]
    public bool ArtifactGroupOn { get; set; }
    public string ArtifactGroupValue { get; set; }

    [Argument("artifact-name", OpcoesDeCliente)]
    [Description(
      "Nome do artefato em PascalCase."
    )]
    public bool ArtifactNameOn { get; set; }
    public string ArtifactNameValue { get; set; }

    [Argument("artifact-version", OpcoesDeCliente)]
    [Description(
      "Versão do artefato no formato:",
      "  major.minor.patch",
      "Exemplo:",
      "  1.0.0"
    )]
    public bool ArtifactVersionOn { get; set; }
    public string ArtifactVersionValue { get; set; }

    [Argument("user", OpcoesDeCliente)]
    [Description(
      "Nome de login no serviço do PackDm."
    )]
    public bool UserOn { get; set; }
    public string UserValue { get; set; }

    [Argument("pass", OpcoesDeCliente)]
    [Description(
      "Senha de login no serviço do PackDm."
    )]
    public bool PassOn { get; set; }
    public string PassValue { get; set; }

    #endregion

    #region Argumentos do servidor...

    [Argument('f', "folder", OpcoesDeServico)]
    [Description(
      "Caminho do repositório de artefatos.",
      "Se omitida a pasta considerada será:",
      "  ~/.pack/repository"
    )]
    public bool FolderOn { get; set; }
    public string FolderValue { get; set; }

    [Argument('p', "port", OpcoesDeServico)]
    [Description(
      "Porta de publicação do serviço.",
      "Por padrão o serviço é publicado na porta: 8585"
    )]
    public bool PortOn { get; set; }
    public string PortValue { get; set; }

    #endregion

    #region Argumentos gerais...

    [Argument("version", OpcoesGerais)]
    [Description(
      "Imprime a versão do aplicativo."
    )]
    public bool VersionOn { get; set; }

    [Argument('h', "help", OpcoesGerais)]
    [Description(
      "Imprime esta ajuda."
    )]
    public bool HelpOn { get; set; }

    [Argument("help-settings", OpcoesGerais)]
    [Description(
      "Imprime as configurações definitivas para o PackDm."
    )]
    public bool HelpSettingsOn { get; set; }

    [Argument("help-pack", OpcoesGerais)]
    [Description(
      "Imprime uma imagem do artefato com suas dependências e seus",
      "arquivos de distribuição."
    )]
    public bool HelpPackOn { get; set; }

    [Argument("help-effective-pack", OpcoesGerais)]
    [Description(
      "Imprime uma imagem do artefato com suas dependências resolvidas",
      "para as versões definitivas e seus arquivos de distribuição."
    )]
    public bool HelpEffectivePackOn { get; set; }

    [Argument("non-interactive", OpcoesGerais)]
    [Description(
      "Modo não-interativo. Não faz perguntas na linha de comando."
    )]
    public bool NonInteractiveOn { get; set; }

    [Argument("verbose", OpcoesGerais)]
    [Description(
      "Exibe mensagens detalhadas de depuração e falhas."
    )]
    public bool VerboseOn { get; set; }

    #endregion

    #region Argumentos avançados...

    [Argument('F', "filter", OpcoesAvancadas)]
    [Description(
      "Filtro de pacotes instalados.",
      "O filtro se aplica apenas aos pacotes declarados no `pack.info`.",
      "O curinga '*' pode ser usado."
    )]
    public bool FilterOn { get; set; }
    public List<string> FilterValue
    {
      get { return filterValue ?? (filterValue = new List<string>()); }
    }
    private List<string> filterValue;

    [Argument("pack-prefix", OpcoesAvancadas)]
    [Description(
      "Define um prefixo para os arquivos de configuração do sistema.",
      "Para a ação `init` o prefixo equivale a definir os arquivos",
      "de configuração, pacote e distribuição.",
      "Por exemplo, o comando:",
      "  init --pack-prefix Sandbox",
      "equivale a:",
      "  init --conf-file Sandbox.pack.conf",
      "       --pack-file Sandbox.pack.info",
      "       --dist-file Sandbox.pack.info",
      "Para as demais ações o prefixo equivale a definir o arquivo",
      "de configuração.",
      "Por exemplo, o comando:",
      "  deploy --pack-prefix Sandbox",
      "equivale a:",
      "  deploy --conf-file Sandbox.pack.conf"
    )]
    public bool PackPrefixOn { get; set; }
    public string PackPrefixValue { get; set; }

    [Argument("conf-file", OpcoesAvancadas)]
    [Description(
      "Nome do arquivo de configuração do PackDm.",
      "Por padrão o nome do arquivo é `pack.conf`."
    )]
    public bool ConfFileOn { get; set; }
    public string ConfFileValue { get; set; }

    [Argument("pack-file", OpcoesAvancadas)]
    [Description(
      "Nome do arquivo de espeficicação do artefato.",
      "Por padrão o nome do arquivo é `pack.info`."
    )]
    public bool PackFileOn { get; set; }
    public string PackFileValue { get; set; }

    [Argument("dist-file", OpcoesAvancadas)]
    [Description(
      "Nome do arquivo de espeficicação do artefato construído.",
      "Por padrão o nome do arquivo é `Dist/pack.info`."
    )]
    public bool DistFileOn { get; set; }
    public string DistFileValue { get; set; }

    [Argument("dist-folder", OpcoesAvancadas)]
    [Description(
      "Nome da pasta contendo os arquivos de distribuição do artefato.",
      "Por padrão o nome da pasta é `Dist`."
    )]
    public bool DistFolderOn { get; set; }
    public string DistFolderValue { get; set; }

    [Argument("deps-folder", OpcoesAvancadas)]
    [Description(
      "Nome da pasta de dependências.",
      "Por padrão o nome da pasta é `Deps`."
    )]
    public bool DepsFolderOn { get; set; }
    public string DepsFolderValue { get; set; }

    [Argument("flat-folder", OpcoesAvancadas)]
    [Description(
      "Desativa a estruturação da pasta em 'Grupo\\Artefato'.",
      "Todos os arquivos e pastas baixados são instalados diretamente",
      "na raiz da pasta 'Deps'"
    )]
    public bool FlatFolderOn { get; set; }

    #endregion

  }
}

