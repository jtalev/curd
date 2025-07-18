using curd.Core.queryBuilder;
using curd.Core.queryParser;

namespace curd.Core.Test.queryParser
{
    public class ParserTest
    {
        [Theory]
        [MemberData(nameof(GetTokenizeData))]
        public void Tokenize(string input, List<Token> result)
        {
            Parser p = new Parser();
            List<Token> tokens = p.Tokenize(input);

            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(tokens[i].literal, result[i].literal);
                Assert.Equal(tokens[i].type, result[i].type);
            }
        }

        public static TheoryData<string, List<Token>> GetTokenizeData()
        {
            return new()
            {
                { 
                    "read(\"stores\").where(\"uuid = '123'\")", 
                    new List<Token>
                    {
                        new Token(Token.keywords["read"], "read"),
                        new Token(TokenType.LBRACKET, "("),
                        new Token(TokenType.STRING, "stores"),
                        new Token(TokenType.RBRACKET, ")"),
                        new Token(TokenType.STOP, "."),
                        new Token(Token.keywords["where"], "where"),
                        new Token(TokenType.LBRACKET, "("),
                        new Token(TokenType.STRING, "uuid = '123'"),
                        new Token(TokenType.RBRACKET, ")")
                    }
                },
                {
                    "read(\"stores\").where(\"uuid = '123'\").order(\"uuid\")",
                    new List<Token>
                    {
                        new Token(Token.keywords["read"], "read"),
                        new Token(TokenType.LBRACKET, "("),
                        new Token(TokenType.STRING, "stores"),
                        new Token(TokenType.RBRACKET, ")"),
                        new Token(TokenType.STOP, "."),
                        new Token(Token.keywords["where"], "where"),
                        new Token(TokenType.LBRACKET, "("),
                        new Token(TokenType.STRING, "uuid = '123'"),
                        new Token(TokenType.RBRACKET, ")"),
                        new Token(TokenType.STOP, "."),
                        new Token(Token.keywords["order"], "order"),
                        new Token(TokenType.LBRACKET, "("),
                        new Token(TokenType.STRING, "uuid"),
                        new Token(TokenType.RBRACKET, ")"),
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GetTransformData))]
        public void Parse(string input, QueryIR result)
        {
            Parser parser = new Parser();
            QueryIR ir = parser.Parse(input);

            Assert.Equal(ir.command, result.command);
            Assert.Equal(ir.tableName, result.tableName);
            for (int i = 0; i < ir.columnNames.Count; i++)
            {
                Assert.Equal(ir.columnNames[i], result.columnNames[i]);
            }
            for (int i = 0; i < ir.values.Count; i++)
            {
                Assert.Equal(ir.values[i].columnName, result.values[i].columnName);
                Assert.Equal(ir.values[i].value, result.values[i].value);
            }
            for (int i = 0; i < ir.clauses.Count; i++)
            {
                Assert.Equal(ir.clauses[i].operatorType, result.clauses[i].operatorType);
                Assert.Equal(ir.clauses[i].condition, result.clauses[i].condition);
            }
        }

        public static TheoryData<string, QueryIR> GetTransformData()
        {
            return new()
            {
                {
                    "read(\"stores\").where(\"uuid = '123'\"",
                    new QueryIR
                    (
                        "read",
                        "stores",
                        new List<string>
                        {

                        },
                        new List<Value>
                        {

                        },
                        new List<Clause>
                        {
                            new Clause( "WHERE", "uuid = '123'")
                        }
                    )
                },
                {
                    "create(\"stores\").values(\"uuid = '123'\", \"store = 'Haymes Geelong West'\")",
                    new QueryIR
                    (
                        "create",
                        "stores",
                        new List<string>
                        {

                        },
                        new List<Value>
                        {
                            new Value( "uuid", "'123'" ),
                            new Value( "store", "'Haymes Geelong West'" ),
                        },
                        new List<Clause>
                        {
                            
                        }
                    )
                },
                {
                    "read(\"stores\").columns(\"uuid\", \"store\").where(\"uuid = '123'\")",
                    new QueryIR
                    (
                        "read",
                        "stores",
                        new List<string>
                        {
                            "uuid",
                            "store"
                        },
                        new List<Value>
                        {

                        },
                        new List<Clause>
                        {
                            new Clause( "WHERE", "uuid = '123'")
                        }
                    )
                },
                {
                    "read(\"stores\").columns(\"uuid\", \"store\").where(\"uuid = '123'\").and(\"uuid = '321'\")",
                    new QueryIR
                    (
                        "read",
                        "stores",
                        new List<string>
                        {
                            "uuid",
                            "store"
                        },
                        new List<Value>
                        {

                        },
                        new List<Clause>
                        {
                            new Clause( "WHERE", "uuid = '123'"),
                            new Clause( "AND", "uuid = '321'")
                        }
                    )
                }
            };
        }
    }
}
