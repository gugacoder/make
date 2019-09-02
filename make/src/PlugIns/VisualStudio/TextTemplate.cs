using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Make.Helpers.Template;

namespace Make.PlugIns.VisualStudio
{
  static class TextTemplate
  {
    /// <summary>
    /// Constrói um texto a partir de um template embarcado no assembly.
    /// Os templates devem existir na pasta "/meta/templates".
    /// </summary>
    /// <param name="templateName">Nome do template embarcado.</param>
    /// <param name="argPairs">
    /// Parâmetros repassados para o template.
    /// Um vetor de chaves e valores.
    /// Nas posições pares devem constar os nomes dos argumentos e nas
    /// posições ímpares devem constar seus valores.
    /// </param>
    /// <returns>O texto construído pela aplicação do template</returns>
    public static string ApplyTemplate(string templateName, params object[] argPairs)
    {
      if ((argPairs.Length % 2) != 0)
        throw new Exception("Quantidade inválida de argumentos. O vetor deveria ter uma coleção de chaves e valores.");

      var template = EmbeddedFiles.RetrieveTextFile(templateName);
      if (template == null)
        throw new Exception("O template não existe embarcado: " + templateName);

      var context = new ExpressionContext();
      for (var i = 0; i < argPairs.Length; i += 2)
      {
        var key = argPairs[i].ToString();
        var value = argPairs[i + 1];
        context[key] = value;
      }

      var expression = ExpressionParser.ParseTemplate(template);
      var html = expression.Evaluate(context);

      return html;
    }
  }
}
