using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Do.Helpers;
using Do.Library.Msi;

namespace Do.PlugIns.Msi
{
  static class MsiTransformationAlgorithm
  {
    public class Transformation
    {
      public string PreRelease { get; set; }

      public string ProductName { get; set; }

      public string ProductTitle { get; set; }

      public int ServicePort { get; set; }

      public string GetProductNameWithSuffix(string productName)
      {
        if (!string.IsNullOrWhiteSpace(productName) && !string.IsNullOrWhiteSpace(PreRelease))
        {
          var suffix = $"-{PreRelease.ToUpper()}";
          if (!productName.EndsWith(suffix))
          {
            return $"{productName}{suffix}";
          }
        }
        return productName;
      }

      public string GetProductTitleWithSuffix(string productTitle)
      {
        if (!string.IsNullOrWhiteSpace(productTitle) && !string.IsNullOrWhiteSpace(PreRelease))
        {
          var suffix = PreRelease.ToUpper();
          if (!productTitle.EndsWith(suffix))
          {
            return $"{productTitle} {suffix}";
          }
        }
        return productTitle;
      }

    }

    public static bool ApplyTransformation(string msiFile, Transformation transformation)
    {
      try
      {
        Prompt.PrintInfo("Transformando MSI...");
        Prompt.PrintInfo(msiFile);

        //
        // Transformando os arquivos MSI
        //
        var applied = TransformMsiFile(msiFile, transformation);
        if (applied)
        {
          // TODO: Mais alguma coisa a fazer?
        }

        return true;
      }
      catch (Exception ex)
      {
        return Prompt.PrintFault(ex, "Impossível transformar o arquivo.");
      }
    }

    private static bool TransformMsiFile(string msiFile, Transformation transformation)
    {
      string newProductName;

      using (var msi = new MsiDatabase(msiFile, DatabaseOpenMode.Transact))
      {
        var oldProductName = msi.ProductName;
        var oldProductCode = msi.ProductCode;

        // Definindo o nome do produto
        transformation.ProductName = transformation.ProductName ?? oldProductName;

        newProductName = 
          transformation.GetProductNameWithSuffix(transformation.ProductName);

        var newProductCode = 
          DeterministicGuid
            .GetDeterministicGuid(newProductName)
            .ToString("B")
            .ToUpper();

        var newUpgradeCode =
          DeterministicGuid
            .GetDeterministicGuid($"{newProductName}-UpgradeCode")
            .ToString("B")
            .ToUpper();

        Prompt.PrintInfo(
            $"Parâmetros\n"
          + $"- ProductName\n"
          + $"  - Anterior: {oldProductName}\n"
          + $"  - Proposto: {newProductName}\n"
          + $"- ProductCode\n"
          + $"  - Anterior: {oldProductCode}\n"
          + $"  - Proposto: {newProductCode}\n"
        );

        if (oldProductName == newProductName && oldProductCode == newProductCode)
        {
          Prompt.PrintInfo("Nada a fazer. O arquivo já está transformado.");
          return false;
        }

        msi.ProductName = newProductName;
        msi.ProductCode = newProductCode;
        msi.UpgradeCode = newUpgradeCode;

        // RevisionNumber é um ID de pacote e deve ser diferente para cada
        // pacote produzido.
        msi.UpdateRevisionNumber();

        msi.Commit();
      }

      return Prompt.PrintInfo($"Arquivo MSI transformado.");
    }

    /// <summary>
    /// Modifica o arquivo Config.System.Diagnostics.xml, que contém configurações de logging.
    /// </summary>
    /// <param name="file">O caminho do arquivo Config.System.Diagnostics.xml.</param>
    /// <param name="transformation">As transformações aplicáveis.</param>
    private static void UpdateLogConfig(string file, Transformation transformation)
    {
      // Propriedades relevantes do arquivo:
      //    <system.diagnostics>
      //      <trace>
      //        <listeners>
      //          <add type="System.Diagnostics.EventLogTraceListener"
      //               initializeData="Trade2"
      //               >
      //        </listeners>
      //      </trace>
      //    </system.diagnostics>

      if (string.IsNullOrWhiteSpace(transformation.ProductName))
      {
        // Nada a fazer
        return;
      }

      var logConfigXml = XDocument.Load(file);

      var initializeDataAttr = (
        from tag in logConfigXml.Descendants()
        from type in tag.Attributes()
        where type.Name.LocalName == "type"
           && type.Value == "System.Diagnostics.EventLogTraceListener"
        from initializeData in tag.Attributes()
        where initializeData.Name.LocalName == "initializeData"
        select initializeData
      ).FirstOrDefault();

      if (initializeDataAttr != null)
      {
        var newProductName = transformation.GetProductNameWithSuffix(
          transformation.ProductName
        );

        initializeDataAttr.Value = newProductName;
        logConfigXml.Save(file);
      }
    }

    /// <summary>
    /// Obtém uma porta única para o sufixo.
    /// Portas de alguns sufixos mais comuns
    /// - alfa  9082
    /// - beta  9090
    /// - trunk 9242
    /// </summary>
    /// <param name="suffix">O sufixo a ser analisado.</param>
    /// <returns>A porta criada para o prefixo.</returns>
    private static int MakeDeterministicPort(string suffix)
    {
      if (string.IsNullOrEmpty(suffix))
        return 90;

      var numbers =
        from ch in suffix.ToLower()
        where ch >= 'a' && ch <= 'z'
        select (int)ch;
      // a conta é arbitrária apenas para forçar a geração
      // da porta 9090 quando o sufixo for beta.
      var port = 9000 - 322 + numbers.DefaultIfEmpty().Sum();
      return port;
    }
  }
}
