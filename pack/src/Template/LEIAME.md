PackDm.Template
===============

Template de arquivos para inicialização de um projeto do PackDm.

Visão Geral
-----------

Para inicializar um projeto do PackDm o inicializador de projetos
desempacota os arquivos contidos nesta pasta diretamente na pasta
destino e define os valores mais apropriados para as propriedades
declaradas neles.

Os templates seguem o formato Schema definido pelo namespace
PackDm.SchemaModel e são eles:

1. pack.conf:
   Arquivo opcional contendo as configurações gerais de uso do PackDm.
   Quando não presente na pasta do projeto as configurações padrão do
   PackDm são assumidas.
2. pack.info:
   Arquivo obrigatório contendo as definições do projeto do PackDm.

Os dois templates devem conter as declarações das propriedades embora
seus valores possam ser omitidos.

O inicializador `PackDm.Algorithms.PackInitializer` deve ser customizado
para definir os valores apropriados para estas propriedades.

Os templates devem ser marcados como `Embedded` para serem embarcados no
assembly.


