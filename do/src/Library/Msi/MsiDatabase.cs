using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Deployment.WindowsInstaller.Linq;

namespace Do.Library.Msi
{
  class MsiDatabase : IDisposable
  {
    private readonly QDatabase database;

    public SummaryInfo SummaryInfo => database.SummaryInfo;

    public MsiDatabase(string msiFile, DatabaseOpenMode mode = DatabaseOpenMode.ReadOnly)
    {
      this.database = new QDatabase(msiFile, mode);
    }

    public string ProductName
    {
      get => GetProperty(nameof(ProductName));
      set => SetProperty(nameof(ProductName), value);
    }

    public string ProductCode
    {
      get => GetProperty(nameof(ProductCode));
      set => SetProperty(nameof(ProductCode), value);
    }

    public string UpgradeCode
    {
      get => GetProperty(nameof(UpgradeCode));
      set => SetProperty(nameof(UpgradeCode), value);
    }

    public string GetProperty(string property)
    {
      var value =
        database.Properties.AsEnumerable()
          .Where(x => x.Property == property)
          .Select(x => x.Value)
          .FirstOrDefault();
      return value;
    }

    public void SetProperty(string name, string value)
    {
      var property = database.Properties.AsEnumerable().First(x => x.Property == name);
      property.Value = value;

      //
      // Triggers
      //
      if (name == "UpgradeCode")
      {
        // Quando a propridade UpgradeCode da tabela Property é modificada
        // a tabela Upgrade precisa ser atualizada também 
        database.Execute($"DELETE FROM Upgrade");
        database.Execute(
          $"INSERT INTO Upgrade (UpgradeCode, VersionMax, Attributes, ActionProperty) " +
          $"VALUES ('{value}', '1.0.0', '0', 'PREVIOUSVERSIONSINSTALLED')"
        );
        database.Execute(
          $"INSERT INTO Upgrade (UpgradeCode, VersionMin, Attributes, ActionProperty) " +
          $"VALUES ('{value}', '1.0.0', '258', 'NEWERPRODUCTFOUND')"
        );
      }
    }

    public void UpdateRevisionNumber()
    {
      database.SummaryInfo.RevisionNumber = Guid.NewGuid().ToString("B").ToUpper();
    }

    private string ToValidGuid(string value)
    {
      if (!value.StartsWith("{"))
      {
        value = $"{{{value}}}";
      }
      return value.ToUpper();
    }

    public void Commit()
    {
      database.Commit();
    }

    public void Dispose()
    {
      database.Dispose();
    }
  }
}
