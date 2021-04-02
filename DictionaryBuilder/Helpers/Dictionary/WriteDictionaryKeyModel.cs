using DictionaryBuilder.Models;
using System.IO;
using System.Text;

namespace DictionaryBuilder
{
    internal sealed partial class DictionaryHelper
    {

        #region " Public Methods "

        /// <summary>
        /// Writes the dictionary keys to a flattened model file in the specified location.
        /// </summary>
        /// <param name="namespace">The namespace of the encapsulated model class.</param>
        /// <param name="filePath">The full path of the file that will be written.</param>
        /// <param name="dictionary">A reference to the <see cref="DictionaryDto"/>.</param>
        public static void WriteDictionaryKeyModel(string @namespace, string filePath, DictionaryDto dictionary)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var sb = new StringBuilder();

            sb.Append("using System;")
              .AppendLine()
              .AppendLine()
              .Append("namespace System")
              .AppendLine()
              .Append("{")
              .AppendLine()
              .Append(Indent).Append("/// <summary>")
              .AppendLine()
              .Append(Indent).Append("/// Provides information about a specific Umbraco dicionary key.")
              .AppendLine()
              .Append(Indent).Append("/// The information includes the key value for the dictionary member.")
              .AppendLine()
              .Append(Indent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append("public struct DictionaryKey")
              .AppendLine()
              .Append(Indent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public readonly string Key;")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public bool HasValue => !string.IsNullOrWhiteSpace(Key);")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public DictionaryKey(string key)")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("{")
              .AppendLine()
              .Append(Indent).Append(Indent).Append(Indent).Append("Key = key;")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("}")
              .AppendLine()
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public static implicit operator DictionaryKey(string key) => new DictionaryKey(key);")
              .AppendLine()
              .Append(Indent).Append(Indent).Append("public static implicit operator string(DictionaryKey dk) => dk.Key;")
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
              .Append(Indent).Append("/// Defines the Umbraco dictionary keys.")
              .AppendLine()
              .Append(Indent).Append("/// </summary>")
              .AppendLine()
              .Append(Indent).Append("public partial class DictionaryKeys")
              .AppendLine()
              .Append(Indent).Append("{")
              .AppendLine();

            foreach (var dictionaryItem in dictionary.OrderedItems)
            {
                sb.Append(Indent)
                  .Append(Indent)
                  .Append($"public static DictionaryKey {dictionaryItem.SanitisedKey()} = \"{dictionaryItem.Key}\";")
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
