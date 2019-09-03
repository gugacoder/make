using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Make
{
  class Disposable : IDisposable
  {
    public event EventHandler Disposed;

    public void Dispose()
    {
      Disposed?.Invoke(this, EventArgs.Empty);
    }
  }
}
