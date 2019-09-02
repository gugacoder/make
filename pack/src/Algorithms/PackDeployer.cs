using System;
using PackDm.Model;
using System.Net;
using System.IO;
using System.Collections.Generic;
using PackDm.Handlers;

namespace PackDm.Algorithms
{
  public class PackDeployer
  {
    public void DeployPack(Uri repositoryUri, FileInfo packFile, DirectoryInfo distFolder)
    {
      var pack = PackHandler.Load(packFile);
      var artifactUri = new Uri(repositoryUri, pack.Id + "/");

      using (var basket = new PackDeployerBasket())
      {
        var packUri = new Uri(artifactUri, Settings.DefaultPackFile);

        foreach (var dist in pack.Dist)
        {
          var filename = dist.Replace('/', Path.DirectorySeparatorChar);
          var filepath = Path.Combine(distFolder.FullName, filename);

          var distFile = new FileInfo(filepath);
          var distUri = new Uri(artifactUri, dist);
          
          basket.Add(distFile, distUri);
        }

        // o descritor do artefato será o último arquivo a ser enviado
        basket.Add(packFile, packUri);

        basket.Complete();
      }
    }
  }
}

