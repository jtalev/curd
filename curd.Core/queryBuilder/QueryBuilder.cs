using System.Text;

namespace curd.Core.queryBuilder
{
    public static class QueryBuilder
    {
        // supported databases
        private const string sqlite = "SQLite";
        private const string mysql = "MySQL";
        private const string sqlserver = "SQL Server";
        private const string postgresql = "PostgreSQL";

        private static readonly Dictionary<string, string> quotedTypes = new()
        {
            { "TEXT", sqlite },
            { "DATETIME", sqlite }
        };

        private static readonly Dictionary<string, string> nonQuotedTypes = new()
        {
            { "INTEGER", sqlite },
            { "REAL", sqlite },
            { "BLOB", sqlite }

        };

        public static string BuildCreateQuery(string tableName, Value[] values)
        {
            StringBuilder query = new StringBuilder($"INSERT INTO {tableName} (");
            
            query.Append(JoinColumnNames(values));

            query.Append(") VALUES (");

            query.Append(JoinColumnValues(values));

            return query.Append(");").ToString();
        }

        public static string BuildReadQuery(string tableName, string[]? columnNames, Clause[]? clauses)
        {
            StringBuilder query = new StringBuilder("SELECT ");

            query.Append(columnNames == null || columnNames.Length == 0
                ? "*"
                : string.Join(", ", columnNames));

            query.Append($" FROM {tableName}");

            if (clauses != null && clauses.Length > 0) {
                query.Append(' ');
                query.Append(JoinClauses(clauses));
            }

            return query.Append(';').ToString();
        }

        public static string BuildUpdateQuery(string tableName, Value[] values, Clause[] clauses)
        {
            StringBuilder query = new StringBuilder($"UPDATE {tableName} SET ");

            for (int i = 0; i < values.Length; i++)
            {
                if (quotedTypes.ContainsKey(values[i].columnType))
                {
                    query.Append($"{values[i].columnName} = '{values[i].value}'");
                }
                else
                {
                    query.Append($"{values[i].columnName} = {values[i].value}");
                }
                if (i < values.Length - 1)
                {
                    query.Append(", ");
                }
            }

            return query.Append($" {JoinClauses(clauses)};").ToString();
        }
    
        public static string BuildDeleteQuery(string tableName, Clause[]? clauses)
        {
            StringBuilder query = new StringBuilder($"DELETE FROM {tableName}");
            
            if (clauses != null)
            {
                query.Append(' ');
                query.Append(JoinClauses(clauses));
            }

            return query.Append(';').ToString();
        }

        private static string JoinColumnNames(Value[] values)
        {
            StringBuilder query = new StringBuilder();
            var columnNames = new List<string>();
            
            foreach (Value v in values)
            {
                columnNames.Add(v.columnName);
            };

            return query.Append(string.Join(", ", columnNames)).ToString();
        }

        private static string JoinColumnValues(Value[] values)
        {
            StringBuilder query = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                if (quotedTypes.ContainsKey(values[i].columnType))
                {
                    query.Append($"'{values[i].value}'");
                }
                else
                {
                    query.Append($"{values[i].value}");
                }
                if (i < values.Length - 1)
                {
                    query.Append(", ");
                }
            }

            return query.ToString();
        }

        private static string JoinClauses(Clause[] clauses)
        {
            StringBuilder query = new StringBuilder();
            
            for (int i = 0; i < clauses.Length; i++)
            {
                query.Append($"{clauses[i].operatorType} {clauses[i].condition}");
                if (i < clauses.Length - 1)
                {
                    query.Append(' ');
                }
            }
            return query.ToString();
        }
    }
}
