namespace curd.Core.queryBuilder
{
    public class Value
    {
        private static readonly string[] supportedColumnTypes = [
            "TEXT",
            "DATETIME",
            "INTEGER",
            "REAL",
            "BLOB"
            ];

        public string columnName;
        public string value;

        public Value(string _columnName, string _value)
        {
            this.columnName = _columnName;
            this.value = _value;
        }
    }
}
