using curd.Core.database;

namespace curd.CLI;

internal class Program
{
    static void Main(string[] args)
    {
        IDatabase database;
        try
        {
            database = DbConfig.InitDatabase();
        }
        catch (Exception e)
        {
            Console.WriteLine(
                $"Error initializing database: {e.Message}\n" +
                $"Check config.json and try again.");
            return;
        }

        Repl repl = new Repl(database);
        repl.Run();

        database.Close();
    }
}