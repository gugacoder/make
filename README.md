DO
==

Ferramenta de compilação, integração e gestão de dependências.

Visão Geral
-----------

O DO gerencia o empacotamento de artefatos, a publicação destes artefatos no
repositório e o download das dependências do projeto.
A ferramenta de linha de comando `do` é capaz tanto de executar as operações
do cliente do repositório quanto lançar o serviço de publicação do repositório.

Instalação e Uso
----------------

1. Baixe o [do-bootstrap](http://keepcoding.net/do/download/)
   para a pasta do projeto e commite este arquivo junto do projeto.
2. Execute o `do-bootstrap` para obter automaticamente uma cópia atualizada
   do do.
3. Execute `do init` para iniciar o projeto.
4. Edit o arquivo `pack.info` e acrescente as dependências do projeto.
5. Execute `do install` para instalar as dependências na pasta `packages\packs`.
6. Execute `do pack deploy` quando o artefato estiver pronto para ser
   publicado no servidor do DO.
   
Repositório Central
-------------------

O repositório central do DO contém os artefatos já publicados e o arquivo de
índice do repositório usado pela ferramenta `do` para resolver dependências.

O repositório do DO está hospedado em:

- [http://keepcoding.net/do/repository/](http://keepcoding.net/do/repository/)

Com uma porta alternativa em:

- [http://keepcoding.net:8585](http://keepcoding.net:8585)

Nomenclatura de Artefato
------------------------

Um artefato do DO é nomeado da seguinte forma:

    Grupo/Artefato/0.0.0
    
Alguns exemplos de `Grupo`:

- `KeepCoding`, para artefatos criados pela própria KeepCoding.

`Artefato` corresponde ao nome do artefato em PascalCase geralmente refletindo
o nome do assembly. Por exemplo;

- `KeepCoding.Sandbox`

`0.0.0` corresponde ao número de versão do artefato no formato:

    major.minor.patch
    
Conforme definição do [Versionamento Semântico](http://semver.org/).
Quando necessário a versão pode ser sucedida de uma nota de release separada por
hífen, por exemplo:

    1.0.0-alpha
    1.0.0-alpha.1
    1.0.0-beta
    1.0.0-beta.1

O arquivo `pack.info`
---------------------

O arquivo `pack.info` contém três definições fundamentais de um artefato do
DO:

- `pack`, O nome do artefato;
- `deps`, A declaração dos nomes das dependências do artefato;
- `dist`, A declaração dos arquivos que compõem o artefato.

Exemplo do arquivo `pack.info`:

    pack
      Sandbox/Exemplo/1.0.0
    deps
      Sandbox/UmArtefato/^1.0.0
      Sandbox/OutroArtefato/~0.1.0
    dist
      LEIAME.md
      **/*.dll
      **/*.exe
      **/*.xml

A endentação dos valores de cada seção em dois espaços é necessária.

Dois curingas são suportados:

- `*` Pode ser usado para indicar qualquer quantidade de caracter na posição.
- `**` Pode ser usado para indicar pastas e subpastas.

Exemplos de uso dos curingas

Todos os arquivos da pasta, não considerando subpastas:

    dist
      *
      
Todos os arquivos da pasta e das subpastas:

    dist
      **
      
Todos os arquivos das subpastas, não considerando a própria pasta:

    dist
      */**
      
Todos os executáveis e todas as DLLs da pasta e subpastas:

    dist
      **/*.exe
      **/*.dll


A ferramenta `do-bootstrap`
---------------------------

O `do-bootstrap` é o comando responsável por manter uma cópia atualizada
da ferramenta `do` na pasta do projeto e pode ser obtido do endereço:

- [http://keepcoding.net/do/download/](http://keepcoding.net/do/download/)

O `do-bootstrap` deve ser copiado para a raiz do projeto que será
gerenciado e pode e deve ser comitado junto com o projeto.

Quando executado diretamente sem parâmetros o `do-bootstrap` copia a
versão mais recente do `do` encontrada nos repositórios do DO.

O arquivo `do` não precisa e não deve ser commitado junto com o projeto.

Para mais informações sobre o `do-bootstrap` use:

    do-bootstrap --help

A ferramenta `do`
-----------------------

O `do` tem duas finalidades básicas:

- Gerenciar as operações de cliente do DO.
- Lançar o servidor de repositório do DO.

As próximas seções tratam dos dois aspectos em separado.

Um detalhamento completo das operações oferecidas pelo `do` pode ser
obtida pela execução:

    do --help
    
Operações do cliente
--------------------

Do ponto de vista do cliente o `do` é usado para:

1. Iniciar o projeto de artefato;
2. Instalar as dependências do artefato;
3. Empacotar o artefato;
4. Publicar o artefato empacotado no repositório do DO.

### 1. Iniciando o projeto do artefato

Comando:

    do init
    
Depois de iniciado dois arquivos são incluídos na pasta do projeto:

- `pack.info`, contendo as definições do artefato
- `pack.conf`, contendo configurações de uso do `do`.

O primeiro arquivo deve ser editado e definido:

- A seção `pack` com o nome do artefato.
- A seção `deps` contendo a declaração de nomes dos artefatos dependentes.

O `do` suporta intervalo de versões para as dependências do artefato.
Para mais detalhes consulte a seção Intervalos de Versão.

### 2. Instalando as dependências do projeto

Comando:

    do install

O `do` é capaz de baixar tanto as dependências declaradas no arquivo
`pack.info` quanto as dependências das dependências recursivamente.
O `do` compora os intervalos de versão entre todos os artefatos
encontrados e tenta determinar a versão mais recente que atende a todos eles.
Apenas uma versão de cada dependência é baixa no fim e publicada na pasta
`packages\packs` do projeto.

### 3. Empacotando o artefato

Comando:

    do pack [ --set-version x.x.x ]
    
O `do` produz uma cópia do arquivo `pack.info` dentro da pasta `dist` do
projeto e indexa na seção `dist` do arquivo todos os arquivos encontrados na
pasta `dist`.

Em geral o comando `do` deve ser executado depois da compilação do projeto
para preparar o pacote do artefato que será submetido ao repositório do DO.

O parâmetro `--set-version` permite modificar a versão do arquivo `pack.info`
copiado. O curinga `x` corresponde pode ser usado para manter valor atual
daquela porção da versão.

Abaixo segue um exemplo da aplicação de `--set-version` para produzir um número
modificado de versão:

  Versão          Máscara         Versão Obtida
  --------------- --------------- ---------------
  `1    `         `2          `   `2.0.0      `
  `1    `         `x.2        `   `1.2.0      `
  `1    `         `x.x.2      `   `1.0.2      `
  `1.4  `         `2          `   `2.4.0      `
  `1.4  `         `x.2        `   `1.2.0      `
  `1.4  `         `x.x.2      `   `1.4.2      `
  `1.4.8`         `2          `   `2.4.8      `
  `1.4.8`         `x.2        `   `1.2.8      `
  `1.4.8`         `x.x.2      `   `1.4.2      `
  `1.4.8`         `3.2.1      `   `3.2.1      `
  `1.4.8`         `x.x.x-alpha`   `1.4.8-alpha`

### 4. Publicando o artefato no repositório do DO

Comando:

    do deploy
 
O `do` publica o arquivo `dist/pack.info` e todos os arquivos declarados
na seção `dist` dele no repositório do DO.

Operações do serviço
--------------------

O `do` oferece duas operações fundamentais para o serviço do DO:

1. Indexador do repositório;
2. Lançador do serviço de publicação do repsitório.

O repositório em geral é estocado na pasta:

    {userdir}/.do/repository
  
Sendo `{userdir}` o caminho da pasta de perfil do usuário.
Embora a pasta possa ser redefinida no arquivo `pack.conf`.

### 1. Indexando o repositório

Comando:

    do index
 
O índice é produzido no arquivo `do.index` na raiz do repositório e contém o
nome completa de cada artefato encontrado no repositório.

Exemplo do arquivo `do.index`:

    #
    # Índice do repositório.
    #
    # 09/06/2016 17:17:22
    #

    Sandbox/Exemplo/1.0.0
    Sandbox/Exemplo/1.0.1
    Sandbox/Exemplo/1.0.2

### 2. Lançando o serviço de publicação do repositório

Comando:

    do serve [--port PORTA] [--folder REPOSITORIO]
    
O serviço é lançado por padrão na porta `8585` mas pode ser refinida na linha de
comando ou no arquivo `pack.conf`.

A pasta do repositório pode também ser redefinida na linha de comando ou no 
arquivo `pack.conf`.


Intervalos de Versão
--------------------

A versão pode ser definida em intervalos:

### 1. De um inicio a um fim

Qualquer versão maior ou igual ao início e menor ou igual ao fim.
Exemplos:

  Versão            Intervalo
  ----------------  ---------------------
  `1 - 2        `   `>= 1.0.0 e <= 2.0.0`
  `1.2 - 3.4    `   `>= 1.2.0 e <= 3.4.0`
  `1.2.3 - 3.4.5`   `>= 1.2.3 e <= 3.4.5`
  
O espaço antes e depois do hífen é fundamental.
  
### 2. Igual, até ou a partir de

Qualquer versão igual a indicada, até a indicada ou a partir da indicada,
segundo os operadores:

-  `=`   Igual à versão indicada.
-  `>`   Maior que a versão indicada.
-  `<`   Menor que a versão indicada.
-  `>=`  Maior ou igual à versão indicada.
-  `<=`  Menor ou igual à versão indicada.

Exemplos:

  Versão          Intervalo
  --------------  -------------------
  `=1     `       `= 1.0.0 `
  `=1.2   `       `= 1.2.0 `
  `=1.2.3 `       `= 1.2.3 `
  `>1     `       `> 1.0.0 `
  `>=1.2  `       `>= 1.2.0`
  `<=1.2.3`       `<= 1.2.3`
      
### 3. Compativel com a versão maior

Qualquer versão superior à versão declarada
até a última versão do primeiro digito não zero.
Exemplos:

  Versão          Intervalo
  --------------  --------------------
  `^1    `        `>= 1.0.0 e < 2.0.0`
  `^1.2  `        `>= 1.2.0 e < 2.0.0`
  `^1.2.3`        `>= 1.2.3 e < 2.0.0`
  `^0.1.2`        `>= 0.1.2 e < 0.2.0`
  `^0.0.1`        `>= 0.0.1 e < 0.0.2`
      
### 4. Compatível com a versão de patch

Qualquer versão compatível com a versão declarada.
A compatibilidade é definida no nível do `patch`.
Exemplos:

  Versão          Intervalo
  --------------  --------------------
  `~1    `        `>= 1.0.0 e < 2.0.0`
  `~1.2  `        `>= 1.2.0 e < 1.3.0`
  `~1.2.3`        `>= 1.2.3 e < 1.3.0`
  `~0.1.2`        `>= 0.1.2 e < 0.2.0`
  `~0.0.1`        `>= 0.0.1 e < 0.1.0`


