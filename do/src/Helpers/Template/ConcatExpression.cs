using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.Helpers.Template
{
  class ConcatExpression : List<IExpression>, IExpression
  {
    public ConcatExpression()
    {
    }

    public string Evaluate(ExpressionContext context)
    {
      var results = this.Select(x => x.Evaluate(context));
      return string.Concat(results);
    }
  }
}
