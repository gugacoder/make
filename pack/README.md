PackDm
======

Gerenciador de dependências simplificado.

Visão Geral
-----------

O PackDm gerencia o empacotamento de artefatos, a publicação destes artefatos no
repositório e o download das dependências do projeto.
A ferramenta de linha de comando `pack` é capaz tanto de executar as operações
do cliente do repositório quanto lançar o serviço de publicação do repositório.

Instalação e Uso
----------------

1. Baixe o [pack-bootstrap.exe](http://keepcoding.net/pack/updater/pack-bootstrap.exe)
   para a pasta do projeto e commite este arquivo junto do projeto.
2. Execute o pack-bootstrap.exe para obter automaticamente uma cópia atualizada
   do pack.exe.
3. Execute `pack.exe init` para iniciar o projeto.
4. Edit o arquivo `pack.info` e acrescente as dependências do projeto.
5. Execute `pack.exe install` para instalar as dependências na pasta `Deps`.
6. Execute `pack.exe pack deploy` quando o artefato estiver pronto para ser
   publicado no servidor do PackDm.
   
Repositório Central
-------------------

O repositório central do PackDm contém os artefatos já publicados e o arquivo de
índice do repositório usado pela ferramenta `pack` para resolver dependências.

O repositório do PackDm está hospedado em:

- [http://keepcoding.net/pack/repository/](http://keepcoding.net/pack/repository/)

Com uma porta alternativa em:

- [http://keepcoding.net:8585](http://keepcoding.net:8585)

Outros recursos relacionados ao PackDm pode ser obtidos em:

- [http://keepcoding.net/pack/](http://keepcoding.net/pack/)

Nomenclatura de Artefato
------------------------

Um artefato do PackDm é nomeado da seguinte forma:

    Grupo/Artefato/0.0.0
    
`Grupo` em geral é um de:

- `KeepCoding`, para artefatos criados pela própria KeepCoding.
- `3rd-party`, para artefatos de terceiros.

`Artefato` corresponde ao nome do artefato em PascalCase geralmente refletindo
o nome do assembly. Por exemplo;

- `PackDm`
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
    etc.

O arquivo `pack.info`
---------------------

O arquivo `pack.info` contém três definições fundamentais de um artefato do
PackDm:

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


A ferramenta `pack-bootstrap.exe`
---------------------------------

O `pack-bootstrap.exe` é o comando responsável por manter uma cópia atualizada
do `pack.exe` na pasta do projeto e pode ser obtido do endereço:

- [http://keepcoding.net/pack/updater/pack-bootstrap.exe](http://keepcoding.net/pack/updater/pack-bootstrap.exe)

O `pack-bootstrap.exe` deve ser copiado para a raiz do projeto que será
gerenciado e pode e deve ser comitado junto com o projeto.

Quando executado diretamente sem parâmetros o `pack-bootstrap.exe` copia a
versão mais recente do `pack.exe` encontrada nos repositórios do PackDm.

O arquivo `pack.exe` não precisa e não deve ser commitado junto com o projeto.

Para mais informações sobre o `pack-bootstrap.exe` use:

    pack-bootstrap.exe --help

A ferramenta `pack.exe`
-----------------------

O `pack.exe` tem duas finalidades básicas:

- Gerenciar as operações de cliente do PackDm.
- Lançar o servidor de repositório do PackDm.

As próximas seções tratam dos dois aspectos em separado.

Um detalhamento completo das operações oferecidas pelo `pack.exe` pode ser
obtida pela execução:

    pack.exe --help
    
Operações do cliente
--------------------

Do ponto de vista do cliente o `pack.exe` é usado para:

1. Iniciar o projeto de artefato;
2. Instalar as dependências do artefato;
3. Empacotar o artefato;
4. Publicar o artefato empacotado no repositório do PackDm.

### 1. Iniciando o projeto do artefato

Comando:

    pack.exe init
    
Depois de iniciado dois arquivos são incluídos na pasta do projeto:

- `pack.info`, contendo as definições do artefato
- `pack.conf`, contendo configurações de uso do `pack.exe`.

O primeiro arquivo deve ser editado e definido:

- A seção `pack` com o nome do artefato.
- A seção `deps` contendo a declaração de nomes dos artefatos dependentes.

O `pack.exe` suporta intervalo de versões para as dependências do artefato.
Para mais detalhes consulte a seção Intervalos de Versão.

### 2. Instalando as dependências do projeto

Comando:

    pack.exe install

O `pack.exe` é capaz de baixar tanto as dependências declaradas no arquivo
`pack.info` quanto as dependências das dependências recursivamente.
O `pack.exe` compora os intervalos de versão entre todos os artefatos
encontrados e tenta determinar a versão mais recente que atende a todos eles.
Apenas uma versão de cada dependência é baixa no fim e publicada na pasta
`Deps` do projeto.

### 3. Empacotando o artefato

Comando:

    pack.exe pack [ --set-version x.x.x ]
    
O `pack.exe` produz uma cópia do arquivo `pack.info` dentro da pasta `Dist` do
projeto e indexa na seção `dist` do arquivo todos os arquivos encontrados na
pasta `Dist`.

Em geral o comando `pack` deve ser executado depois da compilação do projeto
para preparar o pacote do artefato que será submetido ao repositório do PackDm.

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

### 4. Publicando o artefato no repositório do PackDm

Comando:

    pack.exe deploy
 
O `pack.exe` publica o arquivo `Dist/pack.info` e todos os arquivos declarados
na seção `dist` dele no repositório do PackDm.

Operações do serviço
--------------------

O `pack.exe` oferece duas operações fundamentais para o serviço do PackDm:

1. Indexador do repositório;
2. Lançador do serviço de publicação do repsitório.

O repositório em geral é estocado na pasta:

    {userdir}/.pack/repository
  
Sendo `{userdir}` o caminho da pasta de perfil do usuário.
Embora a pasta possa ser redefinida no arquivo `pack.conf`.

### 1. Indexando o repositório

Comando:

    pack.exe index
 
O índice é produzido no arquivo `pack.index` na raiz do repositório e contém o
nome completa de cada artefato encontrado no repositório.

Exemplo do arquivo `pack.index`:

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

    pack.exe serve [--port PORTA] [--folder REPOSITORIO]
    
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


