using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace PackDm.Service
{
  public static class ResourceSerializer
  {
    public static XDocument ToXml(Resource resource)
    {
      using (var memory = new MemoryStream())
      {
        var serializer = new DataContractSerializer(resource.GetType());
        serializer.WriteObject(memory, resource);

        memory.Position = 0;

        var xml = XDocument.Load(memory);
        xml.AddFirst(new XProcessingInstruction("xml-stylesheet", "type=\"text/xsl\" href=\"index.xsl\""));
        return xml;
      }
    }

    public static Stream ToStream(Resource resource)
    {
      var xml = ToXml(resource);

      var memory = new MemoryStream();
      var writer = XmlWriter.Create(memory);

      xml.WriteTo(writer);
      writer.Flush();

      memory.Position = 0;
      return memory;
    }
  }
}

