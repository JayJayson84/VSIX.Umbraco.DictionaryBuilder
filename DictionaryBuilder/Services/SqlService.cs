using DictionaryBuilder.Models;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DictionaryBuilder.Services
{
    internal sealed class SqlService
    {

        #region " Public Methods "

        /// <summary>
        /// Connects to the SQL server and attempts to download and map the dictionary members.
        /// </summary>
        /// <param name="options">A <see cref="VisualStudioOptions"/> instance.</param>
        /// <returns>A <see cref="DictionaryDto"/> representing the dictionary members.</returns>
        public static async Task<DictionaryDto> GetDictionaryAsync(VisualStudioOptions options)
        {
            return await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow("DictionaryBuilder: Connecting to database to retrieve dictionary...");
                await TaskScheduler.Default;

                var dictionary = new DictionaryDto();

                var queryString = @"
                SELECT
	                T1.[id] As 'Id',
	                T1.[key] AS 'Key',
	                T2.[value] As 'Value',
	                T3.[id] AS 'ParentId',
	                T3.[key] AS 'ParentKey'
                FROM [dbo].[cmsDictionary] T1
	                LEFT JOIN [dbo].[cmsLanguageText] T2 ON T1.[id] = T2.[UniqueId] AND T2.[languageId] = 1
	                LEFT JOIN [dbo].[cmsDictionary] T3 ON T3.[id] = T1.[Parent]
                ";
                var connectionString = $"Server={options.Hostname},{options.Port}; Database={options.Database}; User Id={options.Username}; Password={options.GetPassword()};";

                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = null;

                        command = new SqlCommand(queryString, connection);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                dictionary.Add(MapToClass<DictionaryItemDto>(reader));
                            }
                        }

                        return dictionary;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Database error, {ex.Message}.");
                }
            });
        }

        /// <summary>
        /// Connects to the SQL server and attempts to download and map the language members.
        /// </summary>
        /// <param name="options">A <see cref="VisualStudioOptions"/> instance.</param>
        /// <returns>A <see cref="LanguageDto"/> representing the language members.</returns>
        public static async Task<LanguageDto> GetLanguageAsync(VisualStudioOptions options)
        {
            return await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow("DictionaryBuilder: Connecting to database to retrieve languages...");
                await TaskScheduler.Default;

                var language = new LanguageDto();

                var queryString = @"
                SELECT
	                [id] As 'Id',
	                [languageISOCode] AS 'LanguageISOCode',
	                [languageCultureName] As 'LanguageCultureName',
	                [isDefaultVariantLang] AS 'IsDefaultVariantLang'
                FROM [dbo].[umbracoLanguage]
                ";
                var connectionString = $"Server={options.Hostname},{options.Port}; Database={options.Database}; User Id={options.Username}; Password={options.GetPassword()};";

                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = null;

                        command = new SqlCommand(queryString, connection);
                        await connection.OpenAsync();

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                language.Add(MapToClass<LanguageItemDto>(reader));
                            }
                        }

                        return language;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Database error, {ex.Message}.");
                }
            });
        }

        /// <summary>
        /// Maps <see cref="SqlDataReader"/> values to a generic class instance.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="reader">The <see cref="SqlDataReader"/> instance.</param>
        /// <returns>Returns a generic class instance with populated property values, if available. Otherwise a default instance.</returns>
        public static T MapToClass<T>(SqlDataReader reader) where T : class
        {
            try
            {
                T model = Activator.CreateInstance<T>();

                var modelProperties = model
                    .GetType()
                    .GetProperties()
                    .OrderBy(p => p.MetadataToken)
                    .ToList();

                for (int i = 0; i < modelProperties.Count; i++)
                {
                    object value;
                    var type = modelProperties[i].PropertyType;

                    value = type == typeof(bool)
                        ? reader.GetBoolean(i)
                        : reader.GetValue(i);

                    var convertedValue = value == DBNull.Value
                        ? null
                        : type == typeof(Guid?)
                            ? Guid.Parse(value.ToString())
                            : Convert.ChangeType(value, type);

                    modelProperties[i].SetValue(model, convertedValue, null);
                }

                return model;
            }
            catch
            {
                return default;
            }
        }

        #endregion

    }
}
