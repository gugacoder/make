using System;
using PackDm.Model;

namespace PackDm
{
  public class Context
  {
    public Pack Pack { get; set; }
    public Options Options { get; set; }
    public Settings Settings { get; set; }
    public FileSystem FileSystem { get; set; }
    public Credentials Credentials { get; set; }
  }
}

