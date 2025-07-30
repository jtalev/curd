using curd.CLI.display;
using curd.Core.database;
using System.Data;
using Spectre.Console;
using curd.Core.queryParser;
using curd.Core.queryBuilder;

namespace curd.CLI
{
    internal class Repl
    {
        public IDatabase database;

        public Repl(IDatabase _database)
        {
            this.database = _database;
        }

        public void Run()
        {
            while (true)
            {
                AnsiConsole.Write(new Markup("[lightpink4]curd>> [/]"));
                string? input = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(input)) 
                {
                    Console.WriteLine("Enter a query\n");
                    AnsiConsole.Write(new Markup("[lightpink4]curd>> [/]"));
                    input = Console.ReadLine();
                }

                Parser parser = new Parser();
                QueryIR queryIR = parser.Parse(input);

                ExecuteCommand(queryIR);
            }
        }

        private static void WriteCommandOutcomeMessage(int result, string command, string tablename)
        {
            AnsiConsole.Write(
                new Markup(
                    $"* [yellow4_1]{result}[/] record {command} in [yellow4_1]{tablename}[/]\n\n"
                    )
            );
        }

        private int ExecuteNonQueryCommand(QueryIR queryIR)
        {
            string query = QueryBuilder.BuildQuery(queryIR);
            return database.ExecuteNonQueryCommand(query);
        }

        private void ProcessNonQueryCommand(QueryIR queryIR)
        {
            int result = ExecuteNonQueryCommand(queryIR);
            WriteCommandOutcomeMessage(result, queryIR.command, queryIR.tableName);
        }

        private void ExecuteCommand(QueryIR queryIR)
        {
            try
            {
                switch (queryIR.command)
                {
                    case "create":
                        ProcessNonQueryCommand(queryIR);
                        break;
                    case "read":
                        string query = QueryBuilder.BuildQuery(queryIR);
                        DataTable dataTable = database.ExecuteQueryCommand(query);
                        Table table = TableComponent.DisplayQueryResult(dataTable);
                        AnsiConsole.Write(table);
                        break;
                    case "update":
                        ProcessNonQueryCommand(queryIR);
                        break;
                    case "delete":
                        ProcessNonQueryCommand(queryIR);
                        break;
                    case "template":
                        dataTable = database.GetTableInfo(queryIR.tableName);
                        if (queryIR.columnNames.Count == 0)
                        {
                            foreach (DataRow row in dataTable.Rows)
                            {
                                var columnName = row.ItemArray[0] as string;
                                if (columnName != null)
                                {
                                    queryIR.columnNames.Add(columnName);
                                }
                            }
                        }

                        PromptEditor pe = new PromptEditor();
                        string input = pe.EditPrompt(TemplateDslQuery.BuildTemplateQuery(queryIR));
                        Parser parser = new Parser();
                        ExecuteCommand(parser.Parse(input));
                        
                        break;
                    case "test":
                        // a command to use for testing new features
                        pe = new PromptEditor();
                        pe.EditPrompt("COMMAND(\"employee\")");
                        break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine($"Query not supported: {ex.Message}");
            }
        }
    }
}
