using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Do.Helpers.Template
{
  interface IExpression
  {
    string Evaluate(ExpressionContext context);
  }
}
