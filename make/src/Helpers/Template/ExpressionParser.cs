using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Make.Helpers.Template
{
  class ExpressionParser
  {
    public static IExpression ParseTemplateFile(string filePath)
    {
      var template = File.ReadAllText(filePath);
      var expr = ParseTemplate(template);
      return expr;
    }

    public static IExpression ParseTemplate(string template)
    {
      template = Escape(template);
      return CreateExpression(template);
    }

    private static string Escape(string text)
    {
      text =
        Regex.Replace(
          Regex.Replace(text, "{{([^{])", "[sta-cmd]$1"),
          "([^}])}}", "$1[end-cmd]"
        );

      text =
        text
          .Replace("{", "[sta-bkt]")
          .Replace("}", "[end-bkt]")
          .Replace("[sta-cmd]", "{")
          .Replace("[end-cmd]", "}");

      return text;
    }

    private static string Unescape(string text)
    {
      text =
        text
          .Replace("[sta-bkt]", "{")
          .Replace("[end-bkt]", "}");
      return text;
    }

    private static IExpression CreateExpression(string template)
    {
      Match match;
      RegexOptions options = RegexOptions.Singleline;

      match = Regex.Match(template, @"^[\w\d.]*$");
      if (match.Success)
      {
        return new VarExpression(template);
      }

      match = Regex.Match(template, @"^each\s+([\w\d.]+)\s+as\s+([\w\d.]+)\s(.*)", options);
      if (match.Success)
      {
        var items = match.Groups[1].Value;
        var alias = match.Groups[2].Value;
        var rest = match.Groups[3].Value;

        var body = CreateExpression(rest);
        return new EachExpression(items, alias, body);
      }

      var concat = new ConcatExpression();

      var regex = new Regex(@"\{(([^{}]+|(?<x>\{)|(?<-x>\}))+(?(x)(?!)))\}");
      while (regex.IsMatch(template))
      {
        match = regex.Match(template);
        Group group = match.Groups[0];

        var left = template.Substring(0, group.Index);
        var middle = template.Substring(group.Index + 1, group.Length - 2);
        var right = template.Substring(group.Index + group.Length);

        concat.Add(new TextExpression(Unescape(left)));
        concat.Add(CreateExpression(middle));

        template = right;
      }

      concat.Add(new TextExpression(Unescape(template)));

      return concat;
    }
  }
}
