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

        private void ExecuteCommand(QueryIR queryIR)
        {
            try
            {
                switch (queryIR.command)
                {
                    case "create":
                        string query = QueryBuilder.BuildQuery(queryIR);
                        int result = database.ExecuteNonQueryCommand(query);
                        AnsiConsole.Write(
                            new Markup(
                                $"* [yellow4_1]{result}[/] record created in [yellow4_1]{queryIR.tableName}[/]\n\n"
                                )
                            );
                        break;
                    case "read":
                        query = QueryBuilder.BuildQuery(queryIR);
                        DataTable dataTable = database.ExecuteQueryCommand(query);
                        Table table = TableComponent.DisplayQueryResult(dataTable);
                        AnsiConsole.Write(table);
                        break;
                    case "update":
                        query = QueryBuilder.BuildQuery(queryIR);
                        result = database.ExecuteNonQueryCommand(query);
                        AnsiConsole.Write(
                            new Markup(
                                $"* [yellow4_1]{result}[/] record updated in [yellow4_1]{queryIR.tableName}[/]\n\n"
                                )
                            );
                        break;
                    case "delete":
                        query = QueryBuilder.BuildQuery(queryIR);
                        result = database.ExecuteNonQueryCommand(query);
                        AnsiConsole.Write(
                            new Markup(
                                $"* [yellow4_1]{result}[/] record deleted from [yellow4_1]{queryIR.tableName}[/]\n\n"
                                )
                            );
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

                        LineEditor eb = new LineEditor();
                        string input = eb.EditLine(TemplateDslQuery.BuildTemplateQuery(queryIR));
                        Parser parser = new Parser();
                        ExecuteCommand(parser.Parse(input));
                        
                        break;
                    case "test":
                        // a command to use for testing new features
                        eb = new LineEditor();
                        eb.EditLine("COMMAND(\"employee\")");
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
