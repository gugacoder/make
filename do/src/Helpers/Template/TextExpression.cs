using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.Helpers.Template
{
  class TextExpression : IExpression
  {
    private readonly string text;

    public TextExpression(string text)
    {
      this.text = text;
    }

    public string Evaluate(ExpressionContext context)
    {
      return text;
    }
  }
}
