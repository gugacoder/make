using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Helpers.Template
{
  interface IExpression
  {
    string Evaluate(ExpressionContext context);
  }
}
