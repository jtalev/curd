using Spectre.Console;

namespace curd.CLI.display;

internal class PromptEditor
{
    private int promptOffset = 7;
    private int initialCursorTop;
    private int cursorTop;
    private int cursorLeft;
    private char enteredCharacter;
    private List<char> currentPrompt;

    internal string EditPrompt(string prompt) 
    {
        currentPrompt = prompt.ToList<char>();
        initialCursorTop = Console.CursorTop;
        cursorTop = initialCursorTop;
        int commandOffset = prompt.IndexOf("(");
        AnsiConsole.Write(new Markup("[lightpink4]curd>> [/]"));
        Console.Write(prompt);
        cursorLeft = promptOffset + commandOffset;
        Console.SetCursorPosition(cursorLeft, cursorTop);

        bool looping = true;
        while (looping)
        {
            ConsoleKeyInfo cki = Console.ReadKey(true);

            switch(cki.Key)
            {
                case ConsoleKey.Backspace:
                    DeleteBackwards();
                    break;
                case ConsoleKey.Delete:
                    DeleteForward();
                    break;
                case ConsoleKey.LeftArrow:
                    MoveCursor(-1);
                    break;
                case ConsoleKey.RightArrow:
                    MoveCursor(1);
                    break;
                case ConsoleKey.Enter:
                    looping = false;
                    break;
                case ConsoleKey.UpArrow:
                    break;
                case ConsoleKey.DownArrow:
                    break;
                default:
                    InsertCharacter(cki);
                    break;
            }
        }

        return string.Concat(currentPrompt);
    }


    // Functions called on key press
    private void MoveCursor(int offset)
    {
        int offsetCursor = cursorLeft + offset;
        if (offsetCursor > cursorLeft) 
        {
            IncrementCursor(offset);
        } else
        {
            DecrementCursor(offset);
        }
    }

    private void InsertCharacter(ConsoleKeyInfo input)
    {
        enteredCharacter = input.KeyChar;
        int cursorPosition = CursorPromptPosition();
        currentPrompt.Insert(cursorPosition, enteredCharacter);
        WriteCurrentPrompt();
        MoveCursor(1);
    }

    private void DeleteBackwards()
    {
        int promptPosition = CursorPromptPosition();
        if (promptPosition - 1 >= 0)
        {
            currentPrompt.RemoveAt(promptPosition - 1);
        }
        WriteCurrentPrompt();
        RemoveTrailingChar();
        MoveCursor(-1);
    }

    private void DeleteForward()
    {
        int promptPosition = CursorPromptPosition();
        if (promptPosition < currentPrompt.Count)
        {
            currentPrompt.RemoveAt(promptPosition);
        }
        WriteCurrentPrompt();
        RemoveTrailingChar();
    }




    // Utility Functions
    private void IncrementCursor(int offset)
    {
        int offsetCursor = cursorLeft + offset;
        int lastLineLength = LastLineLength();

        if (IsCursorLastLine() && offsetCursor > lastLineLength)
        {
            cursorLeft = lastLineLength;
        }
        else if (cursorLeft >= Console.BufferWidth - 1)
        {
            cursorLeft = 0;
            cursorTop++;
        }
        else
        {
            cursorLeft++;
        }

        Console.SetCursorPosition(cursorLeft, cursorTop);
    }

    private void DecrementCursor(int offset)
    {
        int offsetCursor = cursorLeft + offset;
        if (cursorTop == initialCursorTop && offsetCursor <= promptOffset)
        {
            cursorLeft = promptOffset;

        }
        else if (cursorLeft <= 0)
        {
            cursorLeft = Console.BufferWidth - 1;
            cursorTop--;
        }
        else
        {
            cursorLeft--;
        }

        Console.SetCursorPosition(cursorLeft, cursorTop);
    }

    private int CursorLinePosition()
    {
        return cursorTop - initialCursorTop;
    }

    private int CursorPromptPosition()
    {
        if (cursorTop == initialCursorTop)
        {
            return cursorLeft - promptOffset;
        }
        else
        {
            int linePosition = CursorLinePosition();
            int temp = linePosition * Console.BufferWidth - promptOffset;
            return temp + cursorLeft;
        }
    }

    private int TotalLines()
    {
        int totalPromptLen = currentPrompt.Count + promptOffset;
        return totalPromptLen / Console.BufferWidth;
    }

    private bool IsCursorLastLine()
    {
        int lines = TotalLines();
        return cursorTop == initialCursorTop + lines;
    }

    private int LastLineLength()
    {
        int totalPromptLen = currentPrompt.Count + promptOffset;
        int lines = TotalLines();
        return lines > 0 ? (totalPromptLen % (Console.BufferWidth * lines)) : totalPromptLen;
    }

    private void WriteCurrentPrompt()
    {
        Console.SetCursorPosition(promptOffset, initialCursorTop);
        Console.Write(string.Concat(currentPrompt));
        Console.SetCursorPosition(cursorLeft, cursorTop);
    }

    private void RemoveTrailingChar()
    {
        Console.SetCursorPosition(LastLineLength(), initialCursorTop + TotalLines());
        Console.Write(" ");
        Console.SetCursorPosition(cursorLeft, cursorTop);
    }
}
