using Microsoft.Data.Sqlite;
using System.Data;

namespace curd.Core.database
{
    public class SqliteDb: IDatabase
    {
        private SqliteConnection? connection;
        public SqliteDb() { }

        public void ConnectDatabase(DbConfig config)
        {
            if (!config.Configs.TryGetValue(config.Active, out DbConnectionConfig? connectionConfig))
                throw new KeyNotFoundException(
                    $"No configuartion found for active database '{config.Active}'");

            if (string.IsNullOrEmpty(connectionConfig.Path))
                throw new InvalidDataException(
                    $"No path found in active database '{config.Active}' configuration");
            string path = connectionConfig.Path;
            connection = new SqliteConnection($"Data Source={path}");
            
            try
            {
                connection.Open();
            }
            catch (SqliteException ex)
            {
                throw new SqliteException(
                    $"Error opening sqlite connection to path: {path}" +
                    $"\nOriginal error message: {ex.Message}", ex.SqliteErrorCode);
            }
                

            string[] patharr = path.Split("\\");
            Console.WriteLine($"Connection to {patharr[^1]} open.\n");
        }

        public void Close()
        {
            connection?.Close();
        }

        public DataTable ExecuteQueryCommand(string query)
        {
            if (connection == null)
            {
                throw new InvalidOperationException(
                    "Database connection is not initialized");
            }

            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "Database connection is not open");
            }

            using var command = connection.CreateCommand();
            command.CommandText = query;
            var table = new DataTable();
            try
            {
                using var reader = command.ExecuteReader();
                table.Load(reader);
            }
            catch (SqliteException)
            {
                throw;
            }

            return table;
        }

        public int ExecuteNonQueryCommand(string query)
        {
            if (connection == null)
            {
                throw new InvalidOperationException(
                    "Database connection is not initialized");
            }

            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "Database connection is not open");
            }

            using var command = connection.CreateCommand();
            command.CommandText = query;

            try
            {
                return command.ExecuteNonQuery();
            } catch (SqliteException)
            {
                throw;
            }
        }

        public DataTable GetTables()
        {
            var query = "SELECT * FROM sqlite_master WHERE type='table'";
            return ExecuteQueryCommand(query);
        }

        public DataTable GetTableInfo(string name)
        {
            var query = $"SELECT name, type\r\nFROM pragma_table_info('{name}');\r\n";
            return ExecuteQueryCommand(query);
        }
    }
}
