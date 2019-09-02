using System;
using System.Collections.Generic;
using System.Linq;

//
// ESTUDO EM ANDAMENTO
//
// Project irá substituir os vários algoritmos de captura dos
// parâmetros do projeto do PackDm e do Subversion, facilitando
// acrescentar posteriormente informações do GIT e fo NuGet.
//

namespace Make
{
  /// <summary>
  /// Representação de execução de uma função sem retorno (void).
  /// 
  /// Permite uma sintaxe padronizada para retorno de funções que não
  /// propagam exceções.
  /// 
  /// É esperado que uma função que retorna Ret não lance exceções.
  /// O método chamador espera que falhas sejam retornadas pelo próprio
  /// objeto Ret.
  /// 
  /// Esquelo geral de uma função implementada com Ret:
  /// 
  ///     public Ret Funcao()
  ///     {
  ///       try
  ///       {
  ///         if (... bem sucedido ...)
  ///         {
  ///           return true;      //- conversao implitica de booliano para Ret
  ///         }
  ///         else
  ///         {
  ///           return false;     //- conversao implitica de booliano para Ret
  ///         }
  ///       }
  ///       catch (Exception ex)
  ///       {
  ///         return ex;          //- conversao implitica de Exception para Ret
  ///       }
  ///     }
  ///     
  /// A checagem do retorno pode ser feita pelas propriedades de Ret ou
  /// pela conversão implícita de Ret para o tipo booliano.
  /// 
  /// Exemplo 1: Mais controle sobre o fluxo
  /// 
  ///     Ret ret = Funcao()
  ///     if (ret.Ok)
  ///     {
  ///       //...
  ///     }
  ///     else
  ///     {
  ///       Debug.WriteLine(string.Join(Environment.NewLine, ret.Errors));
  ///       Debug.WriteLine(ret.Exception);
  ///     }
  ///     
  /// Exemplo 2: Recomendado somente se o detalhe da falha puder ser ignorado
  /// 
  ///     Ret ok = Funcao()
  ///     if (ok)
  ///     {
  ///       // ...
  ///     }
  ///     
  ///     ou
  ///     
  ///     Ret ok = Funcao()
  ///     if (!ok)
  ///     {
  ///       // ...
  ///     }
  ///     
  /// </summary>
  public struct Ret
  {
    private string[] _errors;

    /// <summary>
    /// Verdadeiro quando a função é executada sem erros; Falso caso contrário.
    /// </summary>
    public bool Ok
    {
      get;
      set;
    }

    /// <summary>
    /// Coleção das falhas emitidas pela função.
    /// Quando Ok é falso é garantido que Falhas é diferente de nulo.
    /// </summary>
    public string[] Errors
    {
      get => Ok ? _errors : (_errors ?? (_errors = new string[0]));
      internal set => _errors = value;
    }

    /// <summary>
    /// Pilha de erro em caso de exceção capturada.
    /// </summary>
    public Exception Exception
    {
      get;
      internal set;
    }

    /// <summary>
    /// Emite uma falha com as causas indicadas.
    /// </summary>
    /// <param name="errors">As causas da falha.</param>
    /// <returns>O retorno de função representando a falha.</returns>
    public static Ret Fail(params string[] errors)
    {
      return new Ret
      {
        Ok = false,
        Errors = errors
      };
    }

    /// <summary>
    /// Emite uma falha com as causas indicadas.
    /// </summary>
    /// <param name="causas">As causas da falha.</param>
    /// <param name="exception">A exceção ocorrida.</param>
    /// <returns>O retorno de função representando a falha.</returns>
    public static Ret Fail(IEnumerable<string> errors, Exception exception)
    {
      return new Ret
      {
        Ok = false,
        Errors = errors.ToArray(),
        Exception = exception
      };
    }

    /// <summary>
    /// Conversão implitica de booliano para Ret.
    /// Permite uma sintaxe mais simples com o retorno direto de `true' ou `false'.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       if (feito) 
    ///       {
    ///         return true;
    ///       }
    ///       else
    ///       {
    ///         return false;
    ///       }
    ///     }
    /// </summary>
    /// <param name="ok">
    /// Verdadeiro em caso de retorno de sucesso; Falso caso contrário.
    /// </param>
    public static implicit operator Ret(bool ok)
    {
      return new Ret { Ok = ok };
    }

    /// <summary>
    /// Conversão implitica de Ret para booliano.
    /// Permite uma sintaxe mais simples na comparação de resultado da função.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       //...
    ///     }
    ///     
    ///     var ok = Funcao();
    ///     if (ok)
    ///     {
    ///       //...
    ///     }
    /// </summary>
    /// <param name="ok">
    /// Verdadeiro em caso de retorno de sucesso; Falso caso contrário.
    /// </param>
    public static implicit operator bool(Ret ret)
    {
      return ret.Ok;
    }

    /// <summary>
    /// Conversão implitica de exceção para Ret.
    /// Permite uma sintaxe mais simples com o retorno direto da exceção.
    /// 
    /// Exemplo:
    ///     public Ret Funcao()
    ///     {
    ///       try
    ///       {
    ///         return true;
    ///       }
    ///       catch (Exception ex)
    ///       {
    ///         return ex;
    ///       }
    ///     }
    /// </summary>
    /// <param name="exception">A exceção capturada.</param>
    public static implicit operator Ret(Exception exception)
    {
      var errors = new List<string>();
      var ex = exception;
      do
      {
        errors.Add(ex.Message);
      } while ((ex = ex.InnerException) != null);

      return new Ret
      {
        Ok = false,
        Errors = errors.ToArray(),
        Exception = exception
      };
    }
  }
}