using curd.Core.database;
using System.Data;

namespace curd.CLI
{
    internal class Repl
    {
        public IDatabase _database;

        public Repl(IDatabase database) 
        {
            this._database = database;
        }

        public void Run()
        {
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();

                if (input == "show tables")
                {
                    DataTable table = _database.ShowTables();
                    foreach(DataRow row in table.Rows)
                    {
                        Console.WriteLine($"{row["name"]}");
                    }
                } else if (input == "show table info stores")
                {
                    DataTable table = _database.ShowTableInfo("note");
                    foreach (DataRow row in table.Rows)
                    {
                        Console.WriteLine($"{row["name"]} - {row["type"]}");
                    }
                }
            }
        }
    }
}
