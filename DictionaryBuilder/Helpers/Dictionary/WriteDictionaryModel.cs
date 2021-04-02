using DictionaryBuilder.Models;
using System.IO;
using System.Text;

namespace DictionaryBuilder
{
    internal sealed partial class DictionaryHelper
    {

        #region " Public Methods "

        /// <summary>
        /// Writes the dictionary models to a hierarchial class file in the specified location.
        /// </summary>
        /// <param name="namespace">The namespace of the encapsulated models classes.</param>
        /// <param name="filePath">The full path of the file that will be written.</param>
        /// <param name="dictionary">A reference to the <see cref="DictionaryDto"/>.</param>
        public static void WriteDictionaryModel(string @namespace, string filePath, DictionaryDto dictionary)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var sb = new StringBuilder();

            sb.Append("using System;")
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
              .Append(Indent).Append("public partial class Dictionary")
              .AppendLine()
              .Append(Indent).Append("{")
              .AppendLine();

            foreach (var dictionaryItem in dictionary.RootItems)
            {
                if (dictionary.HasChildren(dictionaryItem))
                {
                    WriteModelClass(sb, dictionary, dictionaryItem);
                }
                else
                {
                    WriteModelProperty(sb, dictionaryItem);
                }
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
