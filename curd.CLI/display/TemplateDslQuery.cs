using curd.Core.queryBuilder;
using curd.Core.queryParser;
using System.Text;

namespace curd.CLI.display
{
    internal static class TemplateDslQuery
    {
        public static string BuildTemplateQuery(QueryIR queryIR)
        {
            StringBuilder output = new StringBuilder(
                $"COMMAND(\"{queryIR.tableName}\").values(");
            string values = BuildColumnValueStrings(queryIR);

            output.Append($"{values}).where(\"condition\")");

            return output.ToString();
        }

        private static string
            BuildColumnValueStrings(QueryIR queryIR)
        {
            StringBuilder values = new StringBuilder();

            for (int i = 0;
                i < queryIR.columnNames.Count;
                i++)
            {
                values.Append($"\"[yellow4_1]{queryIR.columnNames[i]}[/] = \"");
                if (i < queryIR.columnNames.Count - 1)
                {
                    values.Append(", ");
                }
            }

            return values.ToString();
        }
    }
}
