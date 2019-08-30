using System;
using System.Collections.Generic;

//
// ESTUDO EM ANDAMENTO
//
// Project irá substituir os vários algoritmos de captura dos
// parâmetros do projeto do PackDm e do Subversion, facilitando
// acrescentar posteriormente informações do GIT e fo NuGet.
//

namespace Do
{
  /// <summary>
  /// Representação de um retorno de função.
  /// 
  /// Permite uma sintaxe padronizada para retorno de funções que não
  /// propagam exceções.
  /// 
  /// O valor produzido pela função ou a exceção capturada podem ser
  /// lançadas diretamente com o uso das conversões implícitas ativadas
  /// para Ret.
  /// 
  /// É esperado que uma função que retorna Ret não lance exceções.
  /// O método chamador espera que falhas sejam retornadas pelo próprio
  /// objeto Ret.
  /// 
  /// Esquelo geral de uma função implementada com Ret:
  /// 
  ///     public Ret<T> Funcao()
  ///     {
  ///       try
  ///       {
  ///         T resultado = ....
  ///        
  ///         //...
  ///        
  ///         return resultado;   //- conversao implitica de T para Ret<T>
  ///       }
  ///       catch (Exception ex)
  ///       {
  ///         return ex;          //- conversao implitica de Exception para Ret<T>
  ///       }
  ///     }
  ///     
  /// A checagem do retorno pode ser feita pelas propriedades de Ret ou
  /// pela conversão implícita de Ret para o tipo retornado.
  /// 
  /// Exemplo 1: Mais controle sobre o fluxo
  /// 
  ///     Ret<T> ret = Funcao()
  ///     if (ret.Ok)
  ///     {
  ///       T valor = ret.Valor;
  ///     }
  ///     else
  ///     {
  ///       Debug.WriteLine(string.Join(Environment.NewLine, ret.Errors));
  ///       Debug.WriteLine(ret.Exception);
  ///     }
  ///     
  /// Exemplo 2: Se T for um objeto e uma falha puder ser ignorada
  /// 
  ///     T valor = Funcao()
  ///     if (valor != null)
  ///     {
  ///       // ...
  ///     }
  ///     
  /// Exemplo 3: Se T for um struct e uma falha puder ser ignorada
  /// 
  ///     T valor = Funcao()
  ///     if (valor != default(T))
  ///     {
  ///       // ...
  ///     }
  ///     
  /// </summary>
  /// <typeparam name="T">O tipo do dado retornado.</typeparam>
  public struct Ret<T>
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
    /// O valor detornado pela função em caso de sucesso.
    /// O valor existe apenas quando Ok é verdadeiro.
    /// </summary>
    public T Value
    {
      get;
      internal set;
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
    /// Conversão implitica de um dado para Ret<T>.
    /// Permite uma sintaxe mais simples com o retorno direto
    /// do dado produzido pela função.
    /// 
    /// Exemplo:
    ///     public Ret<T> Funcao()
    ///     {
    ///       try
    ///       {
    ///         T resultado = ...
    ///         
    ///         // ...
    /// 
    ///         return resultado; //- conversao implicita de T para Ret<T>
    ///       }
    ///       catch (Exception ex)
    ///       {
    ///         return ex;
    ///       }
    ///     }
    /// </summary>
    /// <param name="value">
    /// O resultado produzido pela função.
    /// </param>
    public static implicit operator Ret<T>(T value)
    {
      return new Ret<T> { Ok = true, Value = value };
    }

    /// <summary>
    /// Conversão implitica de Ret<T> para o dado encapsulado.
    /// Permite uma sintaxe mais simples na obtenção do retorno da função.
    /// 
    /// Exemplo:
    ///     public Ret<T> Funcao()
    ///     {
    ///       T resultado = ....
    /// 
    ///       //...
    /// 
    ///       return resultado;
    ///     }
    ///     
    ///     T resultado = Funcao(); //- Conversao implitica de Ret<T> para T
    /// </summary>
    /// <param name="valor">
    /// O resultado produzido pela função.
    /// </param>
    public static implicit operator T(Ret<T> ret)
    {
      return ret.Value;
    }

    /// <summary>
    /// Conversão implitica de Ret<T> para Ret.
    /// Permite uma sintaxe mais genérica na construção do código.
    /// 
    /// Exemplo:
    ///     public Ret<T> FuncaoA()
    ///     {
    ///       //...
    ///     }
    ///     public Ret<X> FuncaoB()
    ///     {
    ///       //...
    ///     }
    ///     
    ///     Ret ok;
    ///     
    ///     ok = FuncaoA();
    ///     if (!ok)
    ///     {
    ///       ok = FuncaoB();
    ///       if (!ok)
    ///       {
    ///         Debug.WriteLine("...");
    ///       }
    ///     }
    /// </summary>
    /// <param name="ret">
    /// O tipo Ret<T> retornado por uma função.
    /// </param>
    public static implicit operator Ret(Ret<T> ret)
    {
      return new Ret
      {
        Ok = ret.Ok,
        Errors = ret.Errors,
        Exception = ret.Exception
      };
    }

    /// <summary>
    /// Conversão implitica de exceção para Ret<T>.
    /// Permite uma sintaxe mais simples com o retorno direto da exceção.
    /// 
    /// Exemplo:
    ///     public Ret<T> Funcao()
    ///     {
    ///       try
    ///       {
    ///         T resultado = ....
    ///        
    ///         //...
    ///        
    ///         return resultado;
    ///       }
    ///       catch (Exception ex)
    ///       {
    ///         return ex;
    ///       }
    ///     }
    /// </summary>
    /// <param name="exception">A exceção capturada.</param>
    public static implicit operator Ret<T>(Exception exception)
    {
      var errors = new List<string>();
      var ex = exception;
      do
      {
        errors.Add(ex.Message);
      } while ((ex = ex.InnerException) != null);

      return new Ret<T>
      {
        Ok = false,
        Errors = errors.ToArray(),
        Exception = exception
      };
    }
  }
}