using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Helpers.Template
{
  class VarExpression : IExpression
  {
    private readonly string varName;

    public VarExpression(string varName)
    {
      this.varName = varName;
    }

    public string Evaluate(ExpressionContext context)
    {
      var tokens = varName.Split('.');

      var value = context[tokens.First()];

      foreach (var token in tokens.Skip(1))
      {
        if (value == null)
          return null;

        var prop = (
          from x in value.GetType().GetProperties()
          where x.Name.Equals(token, StringComparison.InvariantCultureIgnoreCase)
          select x
        ).FirstOrDefault();

        if (prop == null)
          return null;

        value = prop.GetValue(value);
      }

      return value?.ToString();
    }
  }
}
