using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Helpers.Template
{
  class EachExpression : IExpression
  {
    private readonly string itemsVar;
    private readonly string alias;
    private readonly IExpression body;

    public EachExpression(string items, string alias, IExpression body)
    {
      this.itemsVar = items;
      this.alias = alias;
      this.body = body;
    }

    public string Evaluate(ExpressionContext context)
    {
      var items = (context[itemsVar] as IEnumerable)?.Cast<object>();
      if (items == null)
        return null;

      var builder = new StringBuilder();
      foreach (var item in items)
      {
        var subContext = new ExpressionContext(context);
        subContext[this.alias] = item;
        var text = body.Evaluate(subContext);
        builder.Append(text);
      }

      return builder.ToString();
    }
  }
}
