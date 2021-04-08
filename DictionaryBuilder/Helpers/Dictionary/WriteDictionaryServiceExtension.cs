using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DictionaryBuilder
{
    internal sealed partial class DictionaryHelper
    {

        #region " Public Methods "

        /// <summary>
        /// Writes the dictionary service extension methods to a class file in the specified location.
        /// </summary>
        /// <param name="namespace">The namespace of the encapsulated extension class.</param>
        /// <param name="serviceNamespace">The namespace of the encapsulated service class.</param>
        /// <param name="filePath">The full path of the file that will be written.</param>
        public static void WriteDictionaryServiceExtension(string @namespace, string serviceNamespace, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var sb = new StringBuilder();

            var namespaces = new List<string>() { "System", "System.Globalization", "System.Linq", "Umbraco.Core.Models" };

            if (!string.IsNullOrWhiteSpace(serviceNamespace) && @namespace != serviceNamespace)
            {
                namespaces.Add(serviceNamespace);
                namespaces.Sort();
            }

            foreach (var ns in namespaces)
            {
                sb.AppendLine($"using {ns};");
            }

            sb.AppendLine()
              .Append($"namespace {@namespace}")
              .AppendLine()
              .Append("{")
              .AppendLine()
              .Append(Indent).Append("/// <summary>")
              .AppendLine()
              .Append(Indent).Append("/// Provides extension methods to the <see cref=\"DictionaryService\"/> class.")
              .AppendLine()
              .Append(Indent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append("public static class DictionaryServiceExtensions")
              .AppendLine()
              .Append(Indent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <summary>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// Gets the value of the <paramref name=\"dictionaryItem\"/> with the matching <paramref name=\"culture\"/>.")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <param name=\"dictionaryItem\">An <see cref=\"IDictionaryItem\"/> item.</param>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <param name=\"culture\">Optionally specify the <see cref=\"CultureInfo\"/> of the dictionary value to return.</param>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <returns>A locale <see cref=\"string\"/> for the <paramref name=\"dictionaryItem\"/> with matching <paramref name=\"culture\"/>. Otherwise <see langword=\"null\"/>.</returns>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <remarks>If a <paramref name=\"culture\"/> is not provided, the project's default <see cref=\"UmbracoCulture\"/> will be used.</remarks>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public static string GetLocaleValue(this IDictionaryItem dictionaryItem, CultureInfo culture = null)")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("return GetFormattedLocaleValue(dictionaryItem, null, culture);")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("}")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <summary>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// Gets the formatted value of the <paramref name=\"dictionaryItem\"/> with the matching <paramref name=\"culture\"/>.")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <param name=\"dictionaryItem\">An <see cref=\"IDictionaryItem\"/> item.</param>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <param name=\"formatArgs\">An array of string representation to replace within a composite format string.</param>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <param name=\"culture\">Optionally specify the <see cref=\"CultureInfo\"/> of the dictionary value to return.</param>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <returns>A locale <see cref=\"string\"/> for the <paramref name=\"dictionaryItem\"/> with matching <paramref name=\"culture\"/> and format items replaced by the representation <paramref name=\"formatArgs\"/>. Otherwise <see langword=\"null\"/>.</returns>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("/// <remarks>If a <paramref name=\"culture\"/> is not provided, the project's default <see cref=\"UmbracoCulture\"/> will be used.</remarks>")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public static string GetFormattedLocaleValue(this IDictionaryItem dictionaryItem, string[] formatArgs, CultureInfo culture = null)")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("if (dictionaryItem == null) return null;")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("culture = culture ?? UmbracoCulture.Default;")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("var translation = dictionaryItem.Translations.SingleOrDefault(x => x.Language.CultureInfo.Equals(culture));")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("if (translation == null) return null;")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("return formatArgs == null")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append(Indent).Append("? translation.Value")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append(Indent).Append(": string.Format(translation.Value, formatArgs);")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("}")
              .AppendLine()
              .Append(Indent).Append("}")
              .AppendLine()
              .Append("}")
              .AppendLine();

            File.WriteAllText(filePath, sb.ToString());
        }

        #endregion

    }
}
