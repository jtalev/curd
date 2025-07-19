using System.Data;

namespace curd.Core.database
{
    public interface IDatabase
    {
        public void ConnectDatabase(DbConfig config);
        public void Close();
        public DataTable ExecuteQueryCommand(string query);
        public int ExecuteNonQueryCommand(string query);
        public DataTable GetTables();
        public DataTable GetTableInfo(string name);
    }
}
