PackDm.SchemaModel
==================

Namespace de definição e modelagem dos arquivos no formato Schema.

Visão Geral
-----------

Schema é o nome dado ao formato de arquivo de configuração usado pelo PackDm.

O Schema possui três estruturas:

1. Comentário.
   Iniciado com um símbolo `#`.
2. Propriedade.
   Formada por letras minúsculas, números, hífens, sublinhas e pontos e sem
   espaços antes ou depois do nome.
3. Valor.
   Indentado em dois espaços e formado por quaisquer caracteres.

Considere este arquivo de exemplo no formato Schema:

    #
    # Exemplo de arquivo no formato Schema
    #
    chave
      Valor da chave
    # Exemplo de propriedade com múltiplos valores:
    chave-dois
      # Um primeiro valor:
      Valor Um
      # Um segundo valor:
      Valor Dois
      # Um terceiro valor:
      Valor Três

Desconsiderando-se os comentários este arquivo define duas propriedade:

1. A propriedade chamada `chave` valendo `Valor da chave`.
2. A propriedade chamada `chave-dois` contendo uma lista de valores:
   `Valor Um`, `Valor Dois` e `Valor Três`.

Definição da API
----------------

O namespace PackDm.SchemaModel define:

1. A classe SchemaFile.
   Um modelo geral de representação e manipulação de arquivo no formato Schema
2. A classe Schema.
   Uma abstração do arquivo Schema para esconder a complexidade do formato.
   O aplicativo interessado em manipular propriedades armazenadas em um arquivo
   Schema pode usar esta abstração para focar o esforço nas propriedades e seus
   valores.
3. A classe SchemaSerializer.
   Encarregada de interpretar e produzir arquivos no formato Schema.







