using curd.Core.database;
using Spectre.Console;

namespace curd.CLI;

internal class Program
{
    private static void WriteCurdHeader()
    {
        AnsiConsole.Write(new Markup(@"
[yellow4_1]  ___  __  __  ____  ____[/]  
[yellow4_1] / __)(  )(  )(  _ \(  _ \ [/]
[yellow4_1]( (__  )(__)(  )   / )(_) )[/]
[lightpink4] \___)(______)(_)\_)(____/ [/]
"));
    }

    static void Main(string[] args)
    {
        WriteCurdHeader();

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