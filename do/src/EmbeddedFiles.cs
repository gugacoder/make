using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Do
{
  static class EmbeddedFiles
  {
    /// <summary>
    /// Determina se o arquivo está embarcado.
    /// </summary>
    /// <param name="filename">O nome do arquivo embarcado.</param>
    /// <returns>Verdadeiro se o arquivo existir embarcado, falso caso contrário.</returns>
    public static bool Exists(string filename)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var exists = assembly.GetManifestResourceNames().Any(m => m.EndsWith(filename));
      return exists;
    }

    /// <summary>
    /// Obtém o conteúdo de um arquivo texto embarcado.
    /// Se o arquivo não existir nulo será retornado.
    /// </summary>
    /// <param name="filename">O nome do arquivo embarcado.</param>
    /// <returns>
    /// O conteúdo do arquivo texto ou nulo.
    /// </returns>
    public static string RetrieveTextFile(string filename)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var manifests = assembly.GetManifestResourceNames();

      var manifest = manifests.FirstOrDefault(m => m.EndsWith(filename));
      if (manifest == null)
        return null;

      using (var stream = assembly.GetManifestResourceStream(manifest))
      {
        var reader = new StreamReader(stream);
        var text = reader.ReadToEnd();
        return text;
      }
    }

    /// <summary>
    /// Obtém o conteúdo de um arquivo binário embarcado.
    /// Se o arquivo não existir nulo será retornado.
    /// </summary>
    /// <param name="filename">O nome do arquivo embarcado.</param>
    /// <returns>
    /// O conteúdo do arquivo ou nulo.
    /// </returns>
    public static byte[] RetrieveBinary(string filename)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var manifests = assembly.GetManifestResourceNames();

      var manifest = manifests.FirstOrDefault(m => m.EndsWith(filename));
      if (manifest == null)
        return null;

      using (var stream = assembly.GetManifestResourceStream(manifest))
      using (var memory = new MemoryStream())
      {
        stream.CopyTo(memory);
        var bytes = memory.ToArray();
        return bytes;
      }
    }

    /// <summary>
    /// Copia o arquivo embarcado indicado para o fluxo de saída.
    /// </summary>
    /// <param name="filename">O nome do arquivo embarcado.</param>
    /// <param name="output">O fluxo para escrita do arquivo.</param>
    public static Ret CopyTo(string filename, Stream output)
    {
      var assembly = Assembly.GetExecutingAssembly();
      var manifests = assembly.GetManifestResourceNames();

      var manifest = manifests.FirstOrDefault(m => m.EndsWith(filename));
      if (manifest == null)
        return Ret.Fail($"Arquivo não encontrado: {filename}");

      using (var stream = assembly.GetManifestResourceStream(manifest))
      {
        stream.CopyTo(output);
        output.Flush();
        return true;
      }
    }
  }
}
