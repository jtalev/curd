using System.Data;

namespace curd.Core.database
{
    public interface IDatabase
    {
        public void ConnectDatabase(DbConfig config);
        public void Close();
        public DataTable ExecuteTableCommand(string query);
        public DataTable ShowTables();
        public DataTable ShowTableInfo(string name);
    }
}
