using System.Collections.ObjectModel;

namespace DictionaryBuilder.Models
{
    /// <summary>
    /// Represents a collection of database table constraints.
    /// </summary>
    public sealed class SqlConstraints : Collection<SqlConstraint>
    {
        /// <summary>
        /// Represents a collection of database table constraints.
        /// </summary>
        public SqlConstraints()
        {
            Add(new SqlConstraint
            {
                Name = "FK_cmsDictionary_cmsDictionary_id",
                TableName = "cmsDictionary",
                ColumnName = "parent",
                ReferenceTableName = "cmsDictionary",
                ReferenceColumnName = "id"
            });

            Add(new SqlConstraint
            {
                Name = "FK_cmsLanguageText_cmsDictionary_id",
                TableName = "cmsLanguageText",
                ColumnName = "UniqueId",
                ReferenceTableName = "cmsDictionary",
                ReferenceColumnName = "id"
            });
        }
    }
}
