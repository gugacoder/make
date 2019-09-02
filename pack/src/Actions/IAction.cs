using System;

namespace PackDm.Actions
{
  public interface IAction
  {
    void Proceed(Context context);
  }
}

