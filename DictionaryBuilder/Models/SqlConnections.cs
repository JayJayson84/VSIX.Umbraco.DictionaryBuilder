namespace DictionaryBuilder.Models
{
    public sealed class SqlConnections
    {
        public SqlConnections()
        {
            Development = new SqlCredentials();
            Staging = new SqlCredentials();
            Live = new SqlCredentials();
        }

        public SqlCredentials Development { get; set; }
        public SqlCredentials Staging { get; set; }
        public SqlCredentials Live { get; set; }
    }
}
