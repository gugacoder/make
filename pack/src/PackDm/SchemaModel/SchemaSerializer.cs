using System;
using System.Linq;
using System.Collections.Generic;

namespace PackDm.SchemaModel
{
  public class SchemaSerializer
  {
    public SchemaFile Deserialize(SourceReader source)
    {
      using (source)
      {
        var file = new SchemaFile();
        var reader = source.GetReader();

        string line = null;
        while ((line = reader.ReadLine()) != null)
        {
          if (line.TrimStart().StartsWith("#"))
          {
            var text = line.Trim();
            text = text.Substring(1);
            file.AddComment(text);
          }
          else if (line.Trim().Length == 0)
          {
            file.AddEmptyLine();
          }
          else if (line.StartsWith(" "))
          {
            var value = line.Trim();
            if (value == "(null)")
              value = null;
            file.AddValue(value);
          }
          else
          {
            file.AddProperty(line.Trim());
          }
        }

        return file;
      }
    }

    public void Serialize(SchemaFile file, TargetWriter target)
    {
      using (target)
      {
        var writer = target.GetWriter();
        var items = file.GetItems();

        Func<int, bool> indentFunction = index => false;

        for (var i = 0; i < items.Length; i++)
        {
          var item = items[i];

          var indent = indentFunction.Invoke(i);
          if (indent)
          {
            writer.Write("  ");
          }

          if (item.IsComment)
          {
            writer.Write("#");
            writer.WriteLine(item.Text);
          }
          else if (item.IsValue)
          {
            writer.WriteLine((item.Text == null) ? "(null)" : item.Text);
          }
          else
          {
            writer.WriteLine(item.Text);
          }

          if (item.IsProperty)
          {
            var values = file.GetValues(item.Text);
            var lastItem = values.LastOrDefault() ?? item;
            var lastIndex = file.IndexOf(lastItem);

            indentFunction = index => index <= lastIndex;
          }
            
        }

        writer.Flush();
      }
    }
  }
}

