using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.Helpers.Template
{
  class ExpressionContext
  {
    private readonly ExpressionContext parent;
    private readonly Dictionary<string, object> items;

    public ExpressionContext()
    {
      this.items = new Dictionary<string, object>();
    }

    public ExpressionContext(ExpressionContext parent)
    {
      this.items = new Dictionary<string, object>();
      this.parent = parent;
    }

    public object this[string name]
    {
      get => items.ContainsKey(name) ? items[name] : parent?[name];
      set => items[name] = value;
    }
  }
}
