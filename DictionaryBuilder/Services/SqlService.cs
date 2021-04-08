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
        /// <param name="credentials">The <see cref="SqlCredentials"/> for the connection.</param>
        /// <param name="encryptionMethod">The type of key to use when decrypting the SQL password.</param>
        /// <returns>A <see cref="DictionaryDto"/> representing the dictionary members.</returns>
        public static async Task<DictionaryDto> GetDictionaryAsync(SqlCredentials credentials, EncryptionMethod encryptionMethod)
        {
            return await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow("DictionaryBuilder: Connecting to database to retrieve dictionary...");
                await TaskScheduler.Default;

                var dictionary = new DictionaryDto();

                var connectionString = credentials.GetConnectionString(encryptionMethod);
                var queryString = @"
                SELECT
	                T1.[id] AS 'Id',
	                T1.[key] AS 'Key',
	                T2.[value] AS 'Value',
	                T3.[id] AS 'ParentId',
	                T3.[key] AS 'ParentKey'
                FROM [dbo].[cmsDictionary] T1
	                LEFT JOIN [dbo].[cmsLanguageText] T2 ON T1.[id] = T2.[UniqueId] AND T2.[languageId] = (SELECT [id] FROM [dbo].[umbracoLanguage] WHERE [isDefaultVariantLang] = 1)
	                LEFT JOIN [dbo].[cmsDictionary] T3 ON T3.[id] = T1.[Parent]
                ";

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
        /// <param name="credentials">The <see cref="SqlCredentials"/> for the connection.</param>
        /// <param name="encryptionMethod">The type of key to use when decrypting the SQL password.</param>
        /// <returns>A <see cref="LanguageDto"/> representing the language members.</returns>
        public static async Task<LanguageDto> GetLanguageAsync(SqlCredentials credentials, EncryptionMethod encryptionMethod)
        {
            return await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow("DictionaryBuilder: Connecting to database to retrieve languages...");
                await TaskScheduler.Default;

                var language = new LanguageDto();

                var connectionString = credentials.GetConnectionString(encryptionMethod);
                var queryString = @"
                SELECT
	                [id] As 'Id',
	                [languageISOCode] AS 'LanguageISOCode',
	                [languageCultureName] As 'LanguageCultureName',
	                [isDefaultVariantLang] AS 'IsDefaultVariantLang'
                FROM [dbo].[umbracoLanguage]
                ";

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
        /// Tests if a connection to the SQL server is successful.
        /// </summary>
        /// <param name="credentials">The <see cref="SqlCredentials"/> for the connection.</param>
        /// <param name="encryptionMethod">The type of key to use when decrypting the SQL password.</param>
        /// <returns><see langword="True"/> if the connection is successful. Otherwise <see langword="false"/>.</returns>
        public static async Task<bool> TestConnectionAsync(SqlCredentials credentials, EncryptionMethod encryptionMethod)
        {
            return await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Testing connection to SQL host '{credentials.Hostname}'...");
                await TaskScheduler.Default;

                if (credentials == null) return false;

                var connectionString = credentials.GetConnectionString(encryptionMethod);
                var queryString = "SELECT 1";
                var result = false;

                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        object value;
                        SqlCommand command = null;

                        command = new SqlCommand(queryString, connection);
                        await connection.OpenAsync();
                        value = await command.ExecuteScalarAsync();
                        value = value == DBNull.Value ? null : value;
                        result = Convert.ToInt32(value) == 1;

                        var logMessage = result
                            ? "DictionaryBuilder: Connection passed."
                            : "DictionaryBuilder: Connection failed.";

                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        LogHelper.LogToWindow(logMessage);
                        await TaskScheduler.Default;
                    }
                }
                catch (Exception)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow("DictionaryBuilder: Connection failed.");
                    await TaskScheduler.Default;
                    result = false;
                }

                return result;
            });
        }

        /// <summary>
        /// Connects to the SQL server and attempts to synchronise the dictionary tables.
        /// </summary>
        /// <param name="sourceCredentials">The <see cref="SqlCredentials"/> for the source connection.</param>
        /// <param name="targetCredentials">The <see cref="SqlCredentials"/> for the destination connection.</param>
        /// <param name="encryptionMethod">The type of key to use when decrypting the SQL passwords.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task SynchroniseDictionaryAsync(SqlCredentials sourceCredentials, SqlCredentials targetCredentials, EncryptionMethod encryptionMethod)
        {
            await Task.Run(async () =>
            {
                if (!await TestConnectionAsync(sourceCredentials, encryptionMethod) || !await TestConnectionAsync(targetCredentials, encryptionMethod))
                {
                    throw new ArgumentException("Check the SQL connection preferences in the Options dialog and try again.");
                }

                await DeleteAndReseedTableAsync(targetCredentials, encryptionMethod, "umbracoLanguage");
                await DeleteAndReseedTableAsync(targetCredentials, encryptionMethod, "cmsDictionary");
                await TruncateTableAsync(targetCredentials, encryptionMethod, "cmsLanguageText");

                await BulkCopyTableAsync(sourceCredentials, targetCredentials, encryptionMethod, "umbracoLanguage");
                await BulkCopyTableAsync(sourceCredentials, targetCredentials, encryptionMethod, "cmsDictionary");
                await BulkCopyTableAsync(sourceCredentials, targetCredentials, encryptionMethod, "cmsLanguageText");
            });
        }

        /// <summary>
        /// Connects to the source SQL server and attempts to bulk copy the database table, with the name provided, into the destination SQL server.
        /// </summary>
        /// <param name="sourceCredentials">The <see cref="SqlCredentials"/> for the source connection.</param>
        /// <param name="targetCredentials">The <see cref="SqlCredentials"/> for the destination connection.</param>
        /// <param name="encryptionMethod">The type of key to use when decrypting the SQL passwords.</param>
        /// <param name="tableName">The name of the table to copy.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task BulkCopyTableAsync(SqlCredentials sourceCredentials, SqlCredentials targetCredentials, EncryptionMethod encryptionMethod, string tableName)
        {
            await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Synchronising table [dbo].[{tableName}]...");
                await TaskScheduler.Default;

                var sourceConnectionString = sourceCredentials.GetConnectionString(encryptionMethod);
                var targetConnectionString = targetCredentials.GetConnectionString(encryptionMethod);
                var sourceQueryString = $"SELECT * FROM [dbo].[{tableName}]";

                try
                {
                    using (var sourceConnection = new SqlConnection(sourceConnectionString))
                    {
                        SqlCommand command = null;

                        command = new SqlCommand(sourceQueryString, sourceConnection);

                        await sourceConnection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            using (var targetConnection = new SqlConnection(targetConnectionString))
                            {
                                await targetConnection.OpenAsync();

                                using (var bulkCopy = new SqlBulkCopy(targetConnection))
                                {
                                    bulkCopy.DestinationTableName = $"[dbo].[{tableName}]";
                                    await bulkCopy.WriteToServerAsync(reader);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Database error, {ex.Message}.");
                }
            });
        }

        /// <summary>
        /// Connects to the SQL server and attempts to truncate the table with the name provided.
        /// </summary>
        /// <param name="credentials">The <see cref="SqlCredentials"/> for the connection.</param>
        /// <param name="encryptionMethod">The type of key to use when decrypting the SQL password.</param>
        /// <param name="tableName">The name of the table to truncate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task TruncateTableAsync(SqlCredentials credentials, EncryptionMethod encryptionMethod, string tableName)
        {
            await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Truncating table [dbo].[{tableName}]...");
                await TaskScheduler.Default;

                var connectionString = credentials.GetConnectionString(encryptionMethod);
                var queryString = $"TRUNCATE TABLE [dbo].[{tableName}]";

                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = null;

                        command = new SqlCommand(queryString, connection);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Database error, {ex.Message}.");
                }
            });
        }

        /// <summary>
        /// Connects to the SQL server and attempts to delete all rows from the table with the name provided. Reverts the table seed back to zero.
        /// </summary>
        /// <param name="credentials">The <see cref="SqlCredentials"/> for the connection.</param>
        /// <param name="encryptionMethod">The type of key to use when decrypting the SQL password.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task DeleteAndReseedTableAsync(SqlCredentials credentials, EncryptionMethod encryptionMethod, string tableName)
        {
            await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Truncating table [dbo].[{tableName}]...");
                await TaskScheduler.Default;

                var connectionString = credentials.GetConnectionString(encryptionMethod);
                var queryString = $@"
                    DELETE FROM [dbo].[{tableName}];
                    DBCC CHECKIDENT ('[dbo].[{tableName}]',RESEED,0);
                ";

                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = null;

                        command = new SqlCommand(queryString, connection);

                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
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
