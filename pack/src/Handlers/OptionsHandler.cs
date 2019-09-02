using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using PackDm.Model;
using System.Collections;

namespace PackDm.Handlers
{
  public static class OptionsHandler
  {
    public static Options Parse(string[] args)
    {
      var options = new Options();

      var info = ExtractInfo();
      ArgumentInfo actions = info.SingleOrDefault(x => x.Definition.IsNameless);
      IEnumerable<ArgumentInfo> arguments = info.Where(x => !x.Definition.IsNameless);

      var queue = new Queue<string>(args);
      while (queue.Count > 0)
      {
        var token = queue.Dequeue();
        if (token.StartsWith("-"))
        {
          ArgumentInfo argument = null;

          if (token.StartsWith("--"))
          {
            var name = token.Substring(2);
            argument = arguments.SingleOrDefault(x => x.Definition.LongName == name);
          }
          else
          {
            var name = token.Substring(1).First();
            argument = arguments.SingleOrDefault(x => x.Definition.ShortName == name);
          }

          if (argument == null)
          {
            throw new PackDmException("Argumento não reconhecido: " + token);
          }

          argument.Property.SetValue(options, true, null);
          if (argument.HasValue)
          {
            if (queue.Count == 0)
            {
              throw new PackDmException("O argumento requer um valor: " + token);
            }

            var value = queue.Dequeue();

            var list = argument.ValueProperty.GetValue(options, null) as List<string>;
            if (list != null)
            {
              list.Add(value);
            }
            else
            {
              argument.ValueProperty.SetValue(options, value, null);
            }
          }
        }
        else
        {
          actions.Property.SetValue(options, true, null);
          var currentValue = actions.ValueProperty.GetValue(options, null);
          var newValue = (currentValue + " " + token).Trim();
          actions.ValueProperty.SetValue(options, newValue, null);
        }
      }

      return options;
    }

    public static IEnumerable<ArgumentInfo> ExtractInfo()
    {
      return
        from property in typeof(Options).GetProperties()
        let definition = property.GetCustomAttributes(false).OfType<ArgumentAttribute>().SingleOrDefault()
        let description = property.GetCustomAttributes(false).OfType<DescriptionAttribute>().SingleOrDefault()
        let actions = property.GetCustomAttributes(false).OfType<ActionsAttribute>().SingleOrDefault()
        where definition != null
        select new ArgumentInfo
        {
          Property = property,
          Definition = definition,
          Description = description,
          Actions = actions
        };
    }

    public class ArgumentInfo
    {
      public PropertyInfo Property { get; set; }
      public ArgumentAttribute Definition { get; set; }
      public DescriptionAttribute Description { get; set; }
      public ActionsAttribute Actions { get; set; }

      public bool HasValue
      {
        get
        {
          if (hasValue == null)
          {
            hasValue = (ValueProperty != null);
          }
          return hasValue.Value;
        }
      }
      private bool? hasValue;

      public PropertyInfo ValueProperty
      {
        get
        {
          if (valueProperty == null)
          {
            var type = typeof(Options);
            var propertyName = Property.Name.Replace("On", "Value");
            valueProperty = type.GetProperty(propertyName);
          }
          return valueProperty;
        }
      }
      private PropertyInfo valueProperty;
    }

  }
}

