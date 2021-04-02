using DictionaryBuilder.Models;
using System.Text;

namespace DictionaryBuilder
{
    /// <summary>
    /// Helper methods to compile dictionary services, models and extensions.
    /// </summary>
    internal sealed partial class DictionaryHelper
    {

        #region " Constants "

        public const char Indent = '\t';

        #endregion

        #region " Private Methods "

        private static void WriteModelClass(StringBuilder sb, DictionaryDto dictionary, DictionaryItemDto dictionaryItem, int nestingLevel = 1)
        {
            var nestingIndent = new string(Indent, nestingLevel);

            sb.Append(Indent).Append(nestingIndent).Append("/// <summary>")
              .AppendLine()
              .Append(Indent).Append(nestingIndent).Append($"/// Defines the <see cref=\"{dictionaryItem.SanitisedKey()}\"/> Umbraco dictionary keys.")
              .AppendLine()
              .Append(Indent).Append(nestingIndent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append(nestingIndent).Append($"public partial class {dictionaryItem.SanitisedKey()}")
              .AppendLine()
              .Append(Indent).Append(nestingIndent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(nestingIndent).Append($"public static DictionaryKey Key => DictionaryKeys.{dictionaryItem.SanitisedKey()};")
              .AppendLine();

            foreach (var childDictionaryItem in dictionary.ItemsById(dictionaryItem.Id))
            {
                if (dictionary.HasChildren(childDictionaryItem))
                {
                    WriteModelClass(sb, dictionary, childDictionaryItem, nestingLevel + 1);
                }
                else
                {
                    WriteModelProperty(sb, childDictionaryItem, nestingLevel + 1);
                }
            }

            sb.Append(Indent).Append(nestingIndent).Append("}")
              .AppendLine();
        }

        private static void WriteModelProperty(StringBuilder sb, DictionaryItemDto dictionaryItem, int nestingLevel = 1)
        {
            var nestingIndent = new string(Indent, nestingLevel);

            sb.Append(Indent)
              .Append(nestingIndent)
              .Append($"public static DictionaryKey {dictionaryItem.SanitisedKey()} => DictionaryKeys.{dictionaryItem.SanitisedKey()};")
              .AppendLine();
        }

        #endregion

    }
}
