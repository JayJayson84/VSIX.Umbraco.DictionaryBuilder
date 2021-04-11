namespace DictionaryBuilder.Models
{
    /// <summary>
    /// Represents a database table constraint.
    /// </summary>
    public sealed class SqlConstraint
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ReferenceTableName { get; set; }
        public string ReferenceColumnName { get; set; }
        public string AddConstraintCommandText => $"ALTER TABLE [dbo].[{TableName}] ADD CONSTRAINT {Name} FOREIGN KEY({ColumnName}) REFERENCES [dbo].[{ReferenceTableName}] ({ReferenceColumnName});";
        public string DropConstraintCommandText => $"ALTER TABLE [dbo].[{TableName}] DROP CONSTRAINT {Name};";
    }
}
