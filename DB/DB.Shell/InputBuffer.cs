using System.Collections.Generic;
using System.Text;

namespace DB.Shell
{
    public class InputBuffer
    {
        private readonly StringBuilder stringBuilder = new();
        private readonly Stack<char> bracesStack = new();

        public bool TryGetCommands(string input, out List<string> commands)
        {
            commands = null;

            foreach (var c in input)
            {
                if (!ProcessChar(c)) continue;
                
                (commands ??= new List<string>()).Add(stringBuilder.ToString());
                stringBuilder.Clear();
            }

            return commands != null;
        }

        private bool ProcessChar(char c)
        {
            if (char.IsWhiteSpace(c))
            {
                return false;
            }

            stringBuilder.Append(c);

            switch (c)
            {
                case '{':
                    bracesStack.Push('{');
                    break;
                case '}':
                    bracesStack.Pop();
                    break;
            }

            return bracesStack.Count == 0;
        }
    }
}