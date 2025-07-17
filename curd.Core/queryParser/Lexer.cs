namespace curd.Core.queryParser
{
    public class Lexer
    {
        private string input;
        private int position;
        private int readPosition;
        internal char ch;
        internal char peekedCh;

        public Lexer (string _input)
        {
            input = _input;
            position = 0;
            readPosition = 1;
            ch = input[position];
        }

        internal Token NextToken()
        {
            Token token;
            switch (ch)
            {
                case '"':
                    string literal = ReadString();
                    token = new Token(TokenType.STRING, literal);
                    break;
                case '(':
                    token = new Token(TokenType.LBRACKET, ch.ToString());
                    break;
                case ')':
                    token = new Token(TokenType.RBRACKET, ch.ToString());
                    break;
                case ',':
                    token = new Token(TokenType.COMMA, ch.ToString());
                    break;
                case '.':
                    token = new Token(TokenType.STOP, ch.ToString());
                    break;
                case ' ':
                    IgnoreWhiteSpace();
                    token = NextToken();
                    break;
                case '\0':
                    token = new Token(TokenType.EOF, ch.ToString());
                    break;
                default:
                    literal = ReadIdentifier();
                    string keyword = Token.keywords[literal];
                    token = new Token(keyword, literal);
                    break;
            }

            ReadChar();
            return token;
        }

        internal void ReadChar()
        {
            if (readPosition >= input.Length)
            {
                ch = '\0';
            } else
            {
                ch = input[readPosition];
            }
            position = readPosition;
            readPosition++;
        }

        internal void PeekChar()
        {
            if (readPosition >= input.Length)
            {
                peekedCh = '\0';
            }
            else
            {
                peekedCh = input[readPosition];
            }
        }

        internal string ReadString()
        {
            int startPosition = position + 1;
     
            while (true)
            {
                ReadChar();
                if (ch == '"' || ch == '\0')
                {
                    break;
                }
            }
            return input.Substring(startPosition, position - startPosition);
        }

        internal string ReadIdentifier()
        {
            int startPosition = position;

            while (true)
            {
                ReadChar();
                PeekChar();
                if (!IsLetter(peekedCh))
                {
                    break;
                }
            }

            return input.Substring(startPosition, readPosition - startPosition);
        }

        internal void IgnoreWhiteSpace()
        {
            while (char.IsWhiteSpace(ch))
            {
                ReadChar();
            }
        }

        internal bool IsLetter(char character)
        {
            if (!char.IsLetter(character))
            {
                return false;
            }
            return true;
        }
    }
}
