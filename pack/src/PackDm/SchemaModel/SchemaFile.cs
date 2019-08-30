using System;
using System.Linq;
using System.Collections.Generic;

namespace PackDm.SchemaModel
{
  public class SchemaFile
  {
    public class Item
    {
      public string Text { get; set; }

      public bool IsProperty { get; set; }

      public bool IsValue { get; set; }

      public bool IsComment { get; set; }

      public bool IsEmptyLine { get; set; }
    }

    private List<Item> lines;

    public SchemaFile()
    {
      lines = new List<Item>();
    }

    #region Getters

    public Item GetItem(int index)
    {
      return lines[index];
    }

    public Item GetProperty(string key)
    {
      var item = lines.LastOrDefault(p => p.IsProperty && p.Text == key);
      return item;
    }

    public IEnumerable<Item> GetValues(string key)
    {
      var propertyIndex = IndexOfProperty(key);
      if (propertyIndex >= 0)
      {
        for (var i = (propertyIndex + 1); i < lines.Count; i++)
        {
          var item = lines[i];

          if (item.IsValue)
            yield return item;

          if (item.IsProperty)
            break;
        }
      }
    }

    public Item[] GetItems()
    {
      return lines.ToArray();
    }

    #endregion

    #region Estruturação básica do arquivo...

    public void AddEmptyLine()
    {
      lines.Add(new Item { IsEmptyLine = true });
    }

    public void AddComment(string comment)
    {
      lines.Add(new Item { Text = comment, IsComment = true });
    }

    public void AddProperty(string key)
    {
      lines.Add(new Item { Text = key, IsProperty = true });
    }

    public void AddValue(string value)
    {
      var property = FindProperty(lines.Count);
      if (property == null)
      {
        throw new PackDmException(
          "Não há uma propriedade na posição indicada para receber o valor indicado."
        );
      }

      lines.Add(new Item { Text = value, IsValue = true });
    }

    public void AddValue(string key, string value)
    {
      var property = GetProperty(key);
      if (property == null)
      {
        AddProperty(key);
      }

      var values = GetValues(key);
      var item = values.LastOrDefault() ?? property;

      var index = IndexOf(item);
      InsertValue(index + 1, value);
    }

    public void SetValue(string key, string value)
    {
      var property = GetProperty(key);
      if (property == null)
      {
        AddProperty(key);
      }
      else
      {
        ClearProperty(key);
      }

      AddValue(key, value);
    }

    public void SetValues(string key, string value, params string[] values)
    {
      var property = GetProperty(key);
      if (property == null)
      {
        AddProperty(key);
      }
      else
      {
        ClearProperty(key);
      }

      AddValue(key, value);
      foreach (var item in values)
      {
        AddValue(key, item);
      }
    }

    public void SetValues(string key, IEnumerable<string> values)
    {
      var property = GetProperty(key);
      if (property == null)
      {
        AddProperty(key);
      }
      else
      {
        ClearProperty(key);
      }

      foreach (var value in values)
      {
        AddValue(key, value);
      }
    }

    #endregion 

    #region Métodos de pesquisa..

    public int IndexOf(Item item)
    {
      return lines.IndexOf(item);
    }

    public int IndexOfProperty(string key)
    {
      for (var i = 0; i < lines.Count; i++)
      {
        var item = lines[i];
        if (item.IsProperty && item.Text == key)
          return i;
      }
      return -1;
    }

    public int IndexOfValue(string key, string value)
    {
      var propertyIndex = IndexOfProperty(key);
      if (propertyIndex >= 0)
      {
        for (var i = (propertyIndex + 1); i < lines.Count; i++)
        {
          var item = lines[i];

          if (item.IsValue && item.Text == value)
            return i;
          
          if (item.IsProperty)
            return -1;
        }
      }
      return -1;
    }

    public Item FindProperty(int index)
    {
      var start = (index < lines.Count) ? index : lines.Count - 1;
      for (var i = start; i >= 0; i--)
      {
        var item = lines[i];
        if (item.IsProperty)
          return item;
      }
      return null;
    }

    #endregion 

    #region Inserção em posições específicas...

    public void InsertEmptyLine(int index)
    {
      lines.Insert(index, new Item { IsEmptyLine = true });
    }

    public void InsertComment(int index, string comment)
    {
      lines.Insert(index, new Item { IsComment = true, Text = comment });
    }

    public void InsertProperty(int index, string key)
    {
      lines.Insert(index, new Item { IsProperty = true, Text = key });
    }

    public void InsertValue(int index, string value)
    {
      var property = FindProperty(index);
      if (property == null)
      {
        throw new PackDmException("Não existe uma propriedade na posição indicada: " + index);
      }

      lines.Insert(index, new Item { IsValue = true, Text = value });
    }

    #endregion 

    #region Comentários...

    public void CommentProperty(string key)
    {
      var property = GetProperty(key);
      var values = GetValues(key).ToArray();

      property.IsProperty = false;
      property.IsComment = true;

      foreach (var value in values)
      {
        value.IsValue = false;
        value.IsComment = true;
        value.Text = "  " + value.Text;
      }
    }

    public void CommentValue(string key, string value)
    {
      var index = IndexOfValue(key, value);
      if (index >= 0)
      {
        var item = lines[index];
        item.IsValue = false;
        item.IsComment = true;
        item.Text = "  " + item.Text;
      }
    }

    #endregion 

    #region Remoção de itens...

    public bool RemoveItem(int index)
    {
      if (index < lines.Count)
      {
        var item = lines[index];
        if (item.IsEmptyLine)
          return RemoveEmptyLine(index);
        if (item.IsComment)
          return RemoveComment(index);
        if (item.IsProperty)
          return RemoveProperty(index);
        if (item.IsValue)
          return RemoveValue(index);
      }
      return false;
    }

    public bool RemoveEmptyLine(int index)
    {
      var item = lines[index];
      if (item.IsEmptyLine)
      {
        lines.Remove(item);
        return true;
      }
      return false;
    }

    public bool RemoveComment(int index)
    {
      var item = lines[index];
      if (item.IsComment)
      {
        int start = index;
        int end = start;

        for (var i = start; i < lines.Count; i++)
        {
          var current = lines[i];
          if (!current.IsComment)
            break;

          end = i;
        }

        int count = (end - start) + 1;
        lines.RemoveRange(start, count);

        return true;
      }
      return false;
    }

    public bool RemoveProperty(int index)
    {
      var item = lines[index];
      if (item.IsProperty)
      {
        return RemoveProperty(item.Text);
      }
      return false;
    }

    public bool RemoveProperty(string key)
    {
      var property = GetProperty(key);
      if (property == null)
        return false;

      var values = GetValues(key);
      var lastItem = values.LastOrDefault() ?? property;

      int start = IndexOfProperty(key);
      int end = IndexOf(lastItem);

      int count = (end - start) + 1;
      lines.RemoveRange(start, count);

      return true;
    }

    public bool RemoveValue(int index)
    {
      var item = lines[index];
      if (item.IsValue)
      {
        int start = index;
        int end = start;

        for (var i = end; i >= 0; i--)
        {
          var current = lines[i];
          if (current.IsProperty || current.IsValue)
            break;

          start = i;
        }

        int count = (end - start) + 1;
        lines.RemoveRange(start, count);

        return true;
      }
      return false;
    }

    public bool RemoveValue(string key, string value)
    {
      int index = IndexOfValue(key, value);
      return RemoveValue(index);
    }

    #endregion 

    #region Remoção de itens em massa...

    public void ClearEmptyLines()
    {
      lines.RemoveAll(x => x.IsEmptyLine);
    }

    public void ClearComments()
    {
      lines.RemoveAll(x => x.IsComment);
    }

    public void ClearCommentsAndEmptyLines()
    {
      lines.RemoveAll(x => x.IsComment || x.IsEmptyLine);
    }

    public void ClearProperty(string key)
    {
      var values = GetValues(key);
      if (values.Any())
      {
        var last = values.LastOrDefault();

        var start = IndexOfProperty(key) + 1;
        var end = IndexOf(last);

        int count = (end - start) + 1;
        lines.RemoveRange(start, count);
      }
    }

    #endregion 

    #region Manipulação...

    public static SchemaFile Parse(SourceReader source)
    {
      var serializer = new SchemaSerializer();
      var file = serializer.Deserialize(source);
      return file;
    }

    public void Save(TargetWriter target)
    {
      var serializer = new SchemaSerializer();
      serializer.Serialize(this, target);
    }

    #endregion

  }
}

