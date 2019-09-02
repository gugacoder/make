using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Make.Library.Posix
{
  /// <summary>
  /// Tipo para opção com argumento.
  /// Quando presente na linha de comando:
  /// - On é marcado como verdadeiro.
  /// - O próximo argumento na linha de comando é salvo em Value.
  /// </summary>
  public class OptArg<T> : OptArg
  {
    private object _value;

    internal OptArg()
    {
    }

    internal OptArg(T value)
    {
      this.On = true;
      this.Value = value;
    }

    public T Value
    {
      get { return (_value != null) ? (T)_value : default(T); }
      set { _value = value; }
    }

    public override string Text
    {
      get => _value?.ToString();
      set
      {
        if (value == null)
        {
          _value = null;
        }
        else
        {
          try
          {
            _value = Convert.ChangeType(value, typeof(T));
          }
          catch (Exception ex)
          {
            throw new InvalidUsageException(
              $"O valor indicado não pode ser convertido para {typeof(T).Name}: {value}"
              , ex
            );
          }
        }
      }
    }

    public override string ToString() => Text ?? "(missing)";
    public static implicit operator T(OptArg<T> arg) => arg.Value;
    public static implicit operator OptArg<T>(T value) => new OptArg<T>(value);
    public static implicit operator bool(OptArg<T> arg) => arg.On;
  }
}
