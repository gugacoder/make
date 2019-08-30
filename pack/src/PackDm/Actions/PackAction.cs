using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using PackDm.Model;
using PackDm.Algorithms;

namespace PackDm.Actions
{
  public class PackAction : IAction
  {
    public void Proceed(Context context)
    {
      CreatePack(context);
    }

    private void CreatePack(Context context)
    {
      var pack = context.Pack;
      var options = context.Options;
      var settings = context.Settings;
      var fileSystem = context.FileSystem;

      var versionMask = options.SetVersionOn ? options.SetVersionValue : null;

      var compiler = new PackCompiler();
      compiler.FileSystem = fileSystem;
      compiler.ResolveDistributionEnabled = true;

      var file = compiler.CompilePack(pack, versionMask);
      Console.Write("[ok] ");
      Console.WriteLine(file);

    }

  }
}

