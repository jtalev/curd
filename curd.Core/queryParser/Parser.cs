using curd.Core.queryBuilder;

namespace curd.Core.queryParser
{
    public class IR
    {
        public string? command;
        public string? tableName;
        public List<string> columnNames;
        public List<Value> values;
        public List<Clause> clauses;

        public IR()
        {
            columnNames = new List<string>();
            values = new List<Value>();
            clauses = new List<Clause>();
        }

        internal IR(string? command, string? tableName, List<string> columnNames, List<Value> values, List<Clause> clauses)
        {
            this.command = command;
            this.tableName = tableName;
            this.columnNames = columnNames;
            this.values = values;
            this.clauses = clauses;
        }   
    }

    internal class Parser
    {
        private IR ir;

        public Parser()
        {
            ir = new IR();
        }

        public IR Parse(string input)
        {
            return Transform(Tokenize(input));
        }

        internal List<Token> Tokenize(string input)
        {
            Lexer lexer = new Lexer(input);
            Token token;
            List<Token> tokens = new List<Token>();

            while (true)
            {
                token = lexer.NextToken();
                if (token.type == TokenType.EOF)
                {
                    return tokens;
                }
                tokens.Add(token);
            }
        }

        internal IR Transform(List<Token> tokens)
        {
            Token prevKeyToken = new Token("", "");

            foreach (Token token in tokens)
            {
                switch (token.type)
                {
                    case TokenType.COMMAND:
                        ir.command = token.literal;
                        prevKeyToken = token;
                        break;
                    case TokenType.VALUES:
                        prevKeyToken = token;
                        break;
                    case TokenType.COLUMNS:
                        prevKeyToken = token;
                        break;
                    case TokenType.CLAUSE:
                        prevKeyToken = token;
                        break;
                    case TokenType.STRING:
                        ParseString(token.literal, prevKeyToken);
                        break;
                }
            }

            return ir;
        }

        private readonly Dictionary<string, string> keywordConversions = new Dictionary<string, string>
        {
            { "where", "WHERE" },
            { "order", "ORDER BY" },
            { "and", "AND" },
            { "not", "NOT" },
            { "or", "OR" },
        };  

        internal void ParseString(string value, Token prevKeyToken)
        {
            switch (prevKeyToken.type)
            {
                case TokenType.COMMAND:
                    ir.tableName = value;
                    break;
                case TokenType.VALUES:
                    string[] split = value.Split('=');
                    for (int i = 0; i < split.Length; i++)
                    {
                        split[i] = split[i].Trim();
                    }
                    ir.values.Add(new Value(
                            split[0],
                            split[1]
                        ));
                    break;
                case TokenType.COLUMNS:
                    ir.columnNames.Add(value);
                    break;
                case TokenType.CLAUSE:
                    ir.clauses.Add(new Clause(
                            keywordConversions[prevKeyToken.literal],
                            value
                        ));
                    break;
                default:
                    Console.WriteLine("Unable to parse input. Keyword type not supported.");
                    break;
            }
        }
    }
}