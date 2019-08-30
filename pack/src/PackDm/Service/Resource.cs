using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PackDm.Service
{
  [DataContract(Namespace = "")]
  public class Resource
  {
    [DataMember(Order = 1)]
    public string Name
    {
      get;
      set; 
    }

    [DataMember(Order = 2)]
    public string Path
    {
      get;
      set; 
    }

    [DataMember(Order = 3)]
    public DateTime Date
    {
      get;
      set; 
    }

    [DataMember(Order = 3)]
    public List<Item> Items
    {
      get { return items ?? (items = new List<Item>()); }
      set { items = value; }
    }
    private List<Item> items;

    [DataContract(Name = "Item", Namespace = "")]
    public class Item
    {
      [DataMember(Order = 1)]
      public string Name
      {
        get;
        set; 
      }

      [DataMember(Order = 2)]
      public string Path
      {
        get;
        set; 
      }

      [DataMember(Order = 3)]
      public DateTime Date
      {
        get;
        set; 
      }

      [DataMember(Order = 4)]
      public bool IsFolder
      {
        get;
        set;
      }

      [DataMember(Order = 5)]
      public long LengthInBytes
      {
        get;
        set; 
      }

      [DataMember(Order = 6)]
      public string Length
      {
        get { return FormatBytes(LengthInBytes); }
        set { /* não suportado. nada a fazer. */ }
      }

      private string FormatBytes(long bytes)
      {
        var sufixes = new []{ "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        if (bytes == 0)
          return "0" + sufixes[0];

        var count = Math.Abs(bytes);
        var rate = Convert.ToInt32(Math.Floor(Math.Log(count, 1024)));
        var number = Math.Round(count / Math.Pow(1024, rate), 1);
        return (Math.Sign(bytes) * number).ToString() + sufixes[rate];
      }
    }
  }
}