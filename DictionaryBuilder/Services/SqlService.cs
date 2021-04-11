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
        /// Opens a connection to the SQL server and runs a test command.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection"/> to open.</param>
        /// <returns><see langword="True"/> if the connection and test command succeeds. Otherwise <see langword="false"/>.</returns>
        public static async Task<bool> OpenConnectionAsync(SqlConnection connection)
        {
            return await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Opening connection to SQL host '{connection.DataSource}'...");
                await TaskScheduler.Default;

                if (connection == null) return false;

                var queryString = "SELECT 1";
                bool result;

                try
                {
                    object value;
                    SqlCommand command = null;

                    if (connection.State != System.Data.ConnectionState.Open)
                    {
                        await connection.OpenAsync();
                    }

                    command = new SqlCommand(queryString, connection);
                    value = await command.ExecuteScalarAsync();
                    value = value == DBNull.Value ? null : value;
                    result = Convert.ToInt32(value) == 1;
                }
                catch (Exception)
                {
                    result = false;
                }

                if (!result)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    LogHelper.LogToWindow("DictionaryBuilder: Connection failed.");
                    await TaskScheduler.Default;
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
                var sourceConnectionString = sourceCredentials.GetConnectionString(encryptionMethod);

                using (var sourceConnection = new SqlConnection(sourceConnectionString))
                {
                    if (!await OpenConnectionAsync(sourceConnection))
                    {
                        throw new ArgumentException("Check the SQL connection preferences in the Options dialog and try again.");
                    };

                    var targetConnectionString = targetCredentials.GetConnectionString(encryptionMethod);

                    using (var targetConnection = new SqlConnection(targetConnectionString))
                    {
                        if (!await OpenConnectionAsync(targetConnection))
                        {
                            throw new ArgumentException("Check the SQL connection preferences in the Options dialog and try again.");
                        };

                        var targetCommand = targetConnection.CreateCommand();
                        var targetTransaction = targetConnection.BeginTransaction();

                        targetCommand.Connection = targetConnection;
                        targetCommand.Transaction = targetTransaction;

                        await DropConstraintsAsync(targetCommand, targetCredentials.Constraints);
                        await TruncateTableAsync(targetCommand, "cmsDictionary");
                        await TruncateTableAsync(targetCommand, "cmsLanguageText");
                        await AddConstraintsAsync(targetCommand, targetCredentials.Constraints);

                        try
                        {
                            targetTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            try
                            {
                                targetTransaction.Rollback();
                            }
                            catch (Exception)
                            {
                                throw new Exception($"An error occurred attempting to truncate tables from {targetCredentials.Hostname} and drop contraints. Transaction rollback failed.");
                            }

                            throw new Exception($"An error occurred attempting to truncate tables from {targetCredentials.Hostname} and drop contraints. Transaction rolled back successfully.");
                        }

                        var sourceCommand = sourceConnection.CreateCommand();
                        var sourceTransaction = sourceConnection.BeginTransaction();

                        sourceCommand.Connection = sourceConnection;
                        sourceCommand.Transaction = sourceTransaction;

                        await BulkCopyTableAsync(sourceCommand, targetConnection, "cmsDictionary");
                        await BulkCopyTableAsync(sourceCommand, targetConnection, "cmsLanguageText");

                        try
                        {
                            sourceTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            try
                            {
                                sourceTransaction.Rollback();
                            }
                            catch (Exception)
                            {
                                throw new Exception($"An error occurred attempting to bulk copy tables from {sourceCredentials.Hostname} to {targetCredentials.Hostname}. Transaction rollback failed.");
                            }

                            throw new Exception($"An error occurred attempting to bulk copy tables from {sourceCredentials.Hostname} to {targetCredentials.Hostname}. Transaction rolled back successfully.");
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Attempts to bulk copy the database table, with the name provided, into the destination SQL server.
        /// </summary>
        /// <param name="sourceCommand">An <see cref="SqlCommand"/> provided by an open connection to the source database.</param>
        /// <param name="targetConnection">An opened <see cref="SqlConnection"/> to the destination database.</param>
        /// <param name="tableName">The name of the table to copy.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task BulkCopyTableAsync(SqlCommand sourceCommand, SqlConnection targetConnection, string tableName)
        {
            await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Synchronising table [dbo].[{tableName}]...");
                await TaskScheduler.Default;

                sourceCommand.CommandText = $"SELECT * FROM [dbo].[{tableName}]";
                using (var reader = await sourceCommand.ExecuteReaderAsync())
                {
                    using (var bulkCopy = new SqlBulkCopy(targetConnection))
                    {
                        bulkCopy.DestinationTableName = $"[dbo].[{tableName}]";
                        await bulkCopy.WriteToServerAsync(reader);
                    }
                }
            });
        }

        /// <summary>
        /// Drops a collection of database table <paramref name="constraints"/>.
        /// </summary>
        /// <param name="command">An <see cref="SqlCommand"/> provided by an open connection to the database.</param>
        /// <param name="constraints">The collection of <see cref="SqlConstraints"/> to drop.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task DropConstraintsAsync(SqlCommand command, SqlConstraints constraints)
        {
            await Task.Run(async () =>
            {
                foreach (var constraint in constraints)
                {
                    await DropConstraintAsync(command, constraint);
                }
            });
        }

        /// <summary>
        /// Drops a database table <paramref name="constraint"/>.
        /// </summary>
        /// <param name="command">An <see cref="SqlCommand"/> provided by an open connection to the database.</param>
        /// <param name="constraint">The <see cref="SqlConstraint"/> to drop.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task DropConstraintAsync(SqlCommand command, SqlConstraint constraint)
        {
            await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Dropping constraint '{constraint.Name}'...");
                await TaskScheduler.Default;

                command.CommandText = constraint.DropConstraintCommandText;
                await command.ExecuteNonQueryAsync();
            });
        }

        /// <summary>
        /// Adds a collection of database table <paramref name="constraints"/>.
        /// </summary>
        /// <param name="command">An <see cref="SqlCommand"/> provided by an open connection to the database.</param>
        /// <param name="constraints">The collection of <see cref="SqlConstraints"/> to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task AddConstraintsAsync(SqlCommand command, SqlConstraints constraints)
        {
            await Task.Run(async () =>
            {
                foreach (var constraint in constraints)
                {
                    await AddConstraintAsync(command, constraint);
                }
            });
        }

        /// <summary>
        /// Adds a database table <paramref name="constraint"/>.
        /// </summary>
        /// <param name="command">An <see cref="SqlCommand"/> provided by an open connection to the database.</param>
        /// <param name="constraint">The <see cref="SqlConstraint"/> to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task AddConstraintAsync(SqlCommand command, SqlConstraint constraint)
        {
            await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Adding constraint '{constraint.Name}'...");
                await TaskScheduler.Default;

                command.CommandText = constraint.AddConstraintCommandText;
                await command.ExecuteNonQueryAsync();
            });
        }

        /// <summary>
        /// Attempts to truncate the table with the name provided.
        /// </summary>
        /// <param name="command">An <see cref="SqlCommand"/> provided by an open connection to the database.</param>
        /// <param name="tableName">The name of the table to truncate.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task TruncateTableAsync(SqlCommand command, string tableName)
        {
            await Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                LogHelper.LogToWindow($"DictionaryBuilder: Truncating table [dbo].[{tableName}]...");
                await TaskScheduler.Default;

                command.CommandText = $"TRUNCATE TABLE [dbo].[{tableName}]";
                await command.ExecuteNonQueryAsync();
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
