using Spectre.Console;
using Spectre.Console.Rendering;
using System.Data;

namespace curd.CLI.display
{
    internal static class TableComponent
    {
        public static Table DisplayQueryResult(DataTable data)
        {
            Table table = DefaultTable();

            foreach (DataColumn column in data.Columns)
            {
                table.AddColumn(column.ColumnName);
            }

            foreach (DataRow row in data.Rows)
            {
                List<IRenderable> cells = new List<IRenderable>();
                foreach (var item in row.ItemArray)
                {
                    cells.Add(new Markup(item?.ToString() ?? "[grey]null[/]"));
                }
                table.AddRow(cells);
            }

            return table;
        }

        private static Table DefaultTable()
        {
            Table table = new Table();

            table.SimpleBorder();
            table.Collapse();             

            return table;
        }
    }
}
