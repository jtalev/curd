namespace curd.Core.queryParser;

internal class TokenType
{
    public const string EOF = "EOF";

    public const string COMMA = ",";
    public const string STOP = ".";
    public const string LBRACKET = "(";
    public const string RBRACKET = ")";

    public const string STRING = "STRING";
    public const string COMMAND = "COMMAND";
    public const string VALUES = "VALUES";
    public const string COLUMNS = "COLUMNS";
    public const string CLAUSE = "CLAUSE";
    public const string SQL = "SQL";
        
    // keywords
    //public const string WHERE = "WHERE";
    //public const string ORDER = "ORDER BY";
    //public const string AND = "AND";
    //public const string NOT = "NOT";
    //public const string OR = "OR";
    //public const string CREATE = "INSERT";
    //public const string READ = "SELECT";
    //public const string UPDATE = "UPDATE";
    //public const string DELETE = "DELETE";
}

public class Token
{
    internal static readonly Dictionary<string, string> keywords = new Dictionary<string, string>
    {
        { "create", TokenType.COMMAND },
        { "read", TokenType.COMMAND },
        { "update", TokenType.COMMAND },
        { "delete", TokenType.COMMAND },
        { "values", TokenType.VALUES },
        { "columns", TokenType.COLUMNS },
        { "where", TokenType.CLAUSE },
        { "order", TokenType.CLAUSE },
        { "and", TokenType.CLAUSE },
        { "not", TokenType.CLAUSE },
        { "or", TokenType.CLAUSE },
        { "sql", TokenType.SQL },
        { "template", TokenType.COMMAND }
    };    

    public string type;
    public string literal;

    public Token(string _type, string _literal)
    {
        type = _type;
        literal = _literal;
    }
}
