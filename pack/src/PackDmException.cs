using System;

namespace PackDm
{
  public class PackDmException : Exception
  {
    public PackDmException(string message)
      : base(message)
    {
    }

    public PackDmException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}

