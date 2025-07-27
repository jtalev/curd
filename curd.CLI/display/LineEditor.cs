using Spectre.Console;

namespace curd.CLI.display;

internal class LineEditor
{
    private int promptOffset = 7;
    private int cursorTop;
    private int cursorLeft;
    private char enteredCharacter;
    private string currentString;

    internal string EditLine(string str) 
    {
        currentString = str;
        AnsiConsole.Write(new Markup("[lightpink4]curd>> [/]"));
        AnsiConsole.Write(new Markup(str));

        SetInitialPosition(str);

        bool looping = true;
        while (looping)
        {
            ConsoleKeyInfo cki = Console.ReadKey(true);

            switch(cki.Key)
            {
                case ConsoleKey.Backspace:
                    DeleteCharacter();
                    break;
                case ConsoleKey.LeftArrow:
                    SetCursorPosition(-1);
                    break;
                case ConsoleKey.RightArrow:
                    SetCursorPosition(1);
                    break;
                case ConsoleKey.Enter:
                    looping = false;
                    break;
                default:
                    enteredCharacter = cki.KeyChar;
                    InsertCharacter();
                    break;
            }
        }

        return currentString;
    }

    private void SetInitialPosition(string str)
    {
        cursorTop = Console.CursorTop;
        int commandOffset = str.IndexOf("(");
        cursorLeft = promptOffset + commandOffset;
        Console.SetCursorPosition(cursorLeft, cursorTop);
    }

    // Can handle negative and positive offset
    private void SetCursorPosition(int offset)
    {
        cursorLeft += offset;
        if (cursorLeft < promptOffset)
        {
            return;
        }
        if (cursorLeft >= Console.BufferWidth)
        {
            cursorLeft = 1;
            cursorTop++;
            Console.SetCursorPosition(cursorLeft, cursorTop);
        } else if (cursorLeft <= 0)
        {
            cursorLeft = Console.BufferWidth-1;
            cursorTop--;
            Console.SetCursorPosition(cursorLeft, cursorTop);
        } else
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }

    private void InsertCharacter()
    {
        int strLen = currentString.Length + 1;
        char[] chars = new char[strLen];

        for (int i = 0; i < cursorLeft - promptOffset; i++)
        {
            chars[i] = currentString[i];
        }

        chars[cursorLeft-promptOffset] = enteredCharacter;

        for (int i = cursorLeft-promptOffset+1; i < strLen; i++)
        {
            chars[i] = currentString[i - 1];
        }

        currentString = new string(chars);
        cursorLeft++;

        Draw(currentString, promptOffset);
    }

    private void DeleteCharacter()
    {
        int strLen = currentString.Length - 1;
        char[] chars = new char[strLen];

        for (int i = 0; i < cursorLeft - promptOffset - 1; i++)
        {
            chars[i] = currentString[i];
        }

        for (int i = cursorLeft - promptOffset - 1; i < strLen; i++) 
        {
            chars[i] = currentString[i + 1];
        }

        currentString = new string(chars);
        cursorLeft--;

        Draw(currentString, promptOffset);
        Draw(" ", strLen + promptOffset);
    }

    private void Draw(string str, int leftOffset)
    {
        Console.SetCursorPosition(leftOffset, cursorTop);
        Console.Write(str);
        Console.SetCursorPosition(cursorLeft, cursorTop);
    }
}
