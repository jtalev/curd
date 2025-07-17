using curd.Core.queryParser;

namespace curd.Core.Test.queryParser
{
    public class LexerTest
    {
        [Theory]
        [MemberData(nameof(GetReadStringData))]
        public void ReadString(string input, string result)
        {
            Lexer l = new Lexer(input);
            Assert.Equal(result, l.ReadString());
        }

        public static TheoryData<string, string> GetReadStringData()
        {
            return new()
            {
                { "\"hello\"", "hello" }
            };
        }

        [Theory]
        [MemberData(nameof(GetReadIdentifierData))]
        public void ReadIdentifier(string input, string result)
        {
            Lexer l = new Lexer(input);
            Assert.Equal(result, l.ReadIdentifier());
        }

        public static TheoryData<string, string> GetReadIdentifierData()
        {
            return new()
            {
                { "read()", "read" }
            };
        }

        [Theory]
        [MemberData(nameof(GetIgnoreWhiteSpaceData))]
        public void IgnoreWhiteSpace(string input, char result)
        {
            Lexer l = new Lexer(input);
            l.IgnoreWhiteSpace();
            Assert.Equal(result, l.ch);
        }

        public static TheoryData<string, char> GetIgnoreWhiteSpaceData()
        {
            return new()
            {
                { "     c", 'c' },
                { "     \"uuid = '12343'\"", '"' },
            };
        }

        [Theory]
        [MemberData(nameof(GetNextTokenData))]
        public void NextToken(string input, Token result)
        {
            Lexer l = new Lexer(input);
            Token nextToken = l.NextToken();
            Assert.Equal(result.literal, nextToken.literal);
            Assert.Equal(result.type, nextToken.type);
        }

        public static TheoryData<string, Token> GetNextTokenData()
        {
            return new()
            {
                { "     read", new Token(Token.keywords["read"], "read") },
                { "     \"uuid = '12343'\"", new Token(TokenType.STRING, "uuid = '12343'") },
                { "     .", new Token(TokenType.STOP, ".") },
                { "     ,", new Token(TokenType.COMMA, ",") },
                { "     (\"where uuid = '1232'\")", new Token(TokenType.LBRACKET, "(") },
                { "     )", new Token(TokenType.RBRACKET, ")") },
            };
        }
    }
}
