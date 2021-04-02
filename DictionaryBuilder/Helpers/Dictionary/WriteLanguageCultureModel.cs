using DictionaryBuilder.Models;
using System.IO;
using System.Text;

namespace DictionaryBuilder
{
    internal sealed partial class DictionaryHelper
    {

        #region " Public Methods "

        /// <summary>
        /// Writes the language cultures to a model file in the specified location.
        /// </summary>
        /// <param name="namespace">The namespace of the encapsulated model class.</param>
        /// <param name="filePath">The full path of the file that will be written.</param>
        /// <param name="language">A reference to the <see cref="LanguageDto"/>.</param>
        public static void WriteLanguageCultureModel(string @namespace, string filePath, LanguageDto language)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var defaultLanguage = language.GetDefaultLanguage();

            var sb = new StringBuilder();

            sb.Append("using System;")
              .AppendLine()
              .Append("using System.Globalization;")
              .AppendLine()
              .AppendLine()
              .Append("namespace System")
              .AppendLine()
              .Append("{")
              .AppendLine()
              .Append(Indent).Append("/// <summary>")
              .AppendLine()
              .Append(Indent).Append("/// Provides information about a specific Umbraco language culture.")
              .AppendLine()
              .Append(Indent).Append("/// The information includes the culture code (e.g. en-GB) and the equivalent <see cref=\"Globalization.CultureInfo\"/>.")
              .AppendLine()
              .Append(Indent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append("public struct UmbracoCulture")
              .AppendLine()
              .Append(Indent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append($"public static UmbracoCulture Default => new UmbracoCulture(\"{defaultLanguage.LanguageISOCode}\");")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public readonly string ISO;")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public readonly CultureInfo CultureInfo;")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public UmbracoCulture(string iso)")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("ISO = iso;")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("try { CultureInfo = new CultureInfo(ISO); } catch { CultureInfo = null; }")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("}")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public static implicit operator UmbracoCulture(string iso) => new UmbracoCulture(iso);")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public static implicit operator CultureInfo(UmbracoCulture uc) => uc.CultureInfo;")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public static implicit operator string(UmbracoCulture uc) => uc.ISO;")
              .AppendLine()
              .Append(Indent).Append("}")
              .AppendLine()
              .Append("}")
              .AppendLine()
              .AppendLine()
              .Append($"namespace {@namespace}")
              .AppendLine()
              .Append("{")
              .AppendLine()
              .Append(Indent).Append("/// <summary>")
              .AppendLine()
              .Append(Indent).Append("/// Defines the Umbraco language cultures.")
              .AppendLine()
              .Append(Indent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append("public partial class UmbracoCultures")
              .AppendLine()
              .Append(Indent).Append("{")
              .AppendLine();

            foreach (var languageItem in language)
            {
                sb.Append(Indent).Append(Indent).Append($"public static readonly UmbracoCulture {languageItem.GetSanitisedCultureName()} = \"{languageItem.LanguageISOCode}\";")
                  .AppendLine();
            }

            sb.Append(Indent).Append("}")
              .AppendLine()
              .Append("}")
              .AppendLine();

            File.WriteAllText(filePath, sb.ToString());
        }

        #endregion

    }
}
