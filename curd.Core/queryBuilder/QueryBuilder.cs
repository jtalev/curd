using curd.Core.queryParser;
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

        // dont need these right now, may decide in the future to not require
        // users input single quotes for quoted column types and not for
        // non quoted column types

        //private static readonly Dictionary<string, string> quotedTypes = new()
        //{
        //    { "TEXT", sqlite },
        //    { "DATETIME", sqlite }
        //};

        //private static readonly Dictionary<string, string> nonQuotedTypes = new()
        //{
        //    { "INTEGER", sqlite },
        //    { "REAL", sqlite },
        //    { "BLOB", sqlite }

        //};

        public static string BuildQuery(QueryIR queryIR)
        {
            switch (queryIR.command)
            {
                case "create":
                    return BuildCreateQuery(queryIR.tableName, queryIR.values.ToArray());
                case "read":
                    return BuildReadQuery(
                        queryIR.tableName,
                        queryIR.columnNames.ToArray(),
                        queryIR.clauses.ToArray());
                case "update":
                    return BuildUpdateQuery(
                        queryIR.tableName,
                        queryIR.values.ToArray(),
                        queryIR.clauses.ToArray());
                case "delete":
                    return BuildDeleteQuery(
                        queryIR.tableName,
                        queryIR.clauses.ToArray());
                default:
                    throw new NotSupportedException(
                        $"command '{queryIR}' not supported");
            }
        }

        internal static string BuildCreateQuery(string tableName, Value[] values)
        {
            StringBuilder query = new StringBuilder($"INSERT INTO {tableName} (");
            
            query.Append(JoinColumnNames(values));

            query.Append(") VALUES (");

            query.Append(JoinColumnValues(values));

            return query.Append(");").ToString();
        }

        internal static string BuildReadQuery(string tableName, string[]? columnNames, Clause[]? clauses)
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

        internal static string BuildUpdateQuery(string tableName, Value[] values, Clause[] clauses)
        {
            StringBuilder query = new StringBuilder($"UPDATE {tableName} SET ");

            for (int i = 0; i < values.Length; i++)
            {
                query.Append($"{values[i].columnName} = {values[i].value}");
                if (i < values.Length - 1)
                {
                    query.Append(", ");
                }
            }

            return query.Append($" {JoinClauses(clauses)};").ToString();
        }
    
        internal static string BuildDeleteQuery(string tableName, Clause[]? clauses)
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
                query.Append($"{values[i].value}");
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
