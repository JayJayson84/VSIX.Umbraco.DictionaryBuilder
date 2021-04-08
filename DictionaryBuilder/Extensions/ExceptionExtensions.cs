using System;
using System.Text;

namespace DictionaryBuilder.Extensions
{
    internal static class Exceptions
    {

        /// <summary>
        /// Flattens one or more aggregate exceptions into a single error message.
        /// </summary>
        /// <param name="aggregateExeption">Represents one or more errors that occur during application execution.</param>
        /// <returns>The error message that explains the reason for the aggregated exceptions.</returns>
        public static string AggregateMessage(this AggregateException aggregateExeption)
        {
            var aggregateExceptions = aggregateExeption.Flatten().InnerExceptions;
            
            var sb = new StringBuilder($"DictionaryBuilder: ({aggregateExceptions.Count}) errors occurred:")
                .AppendLine()
                .AppendLine();

            for (var i = 0; i < aggregateExceptions.Count; i++)
            {
                sb.AppendLine($"[Error {i + 1}]")
                  .AppendLine($"{aggregateExceptions[i].GetType()}: {aggregateExceptions[i].Message}")
                  .AppendLine();
            }

            return sb.ToString();
        }

    }
}
