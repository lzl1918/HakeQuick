#define TEST

using HakeQuick.Abstraction.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakeQuick.Implementation.Base
{
    internal static class CharCateHelper
    {
        public static bool IsWhiteSpace(this char ch)
        {
            return " \t\v\n\r".IndexOf(ch) >= 0;
        }
        public static bool IsChar(this char ch)
        {
            if (ch <= 'Z' && ch >= 'A') return true;
            else if (ch <= '9' && ch >= '0') return true;
            else if (ch <= 'z' && ch >= 'a') return true;
            else if ("+*^&#@(){}[].|".IndexOf(ch) >= 0) return true;
            return false;
        }
    }

#if TEST
    public
#else
    internal
#endif
        sealed class Command : ICommand
    {
        public bool ContainsError { get; }

        public string Raw { get; }
        public string Identity { get; }
        public string Action { get; }
        public string ActionPost { get; }
        public Dictionary<string, object> NamedArguments { get; }

        public List<object> UnnamedArguments { get; }


        public Command(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            input = input.ToLower();

            string identity = "";
            string action = "";
            Raw = input;

            Dictionary<string, object> namedargs = new Dictionary<string, object>();
            List<object> unnamedargs = new List<object>();

            int len = input.Length;
            StringBuilder valueBuilder = new StringBuilder(len);
            List<string> valueArray = new List<string>();
            string key = "";

            char ch;
            int index = 0;
            int state = 0;
            bool errorflag = false;
            bool enterNamedArg = false;
            while (true)
            {
                if (index < len)
                    ch = input[index];
                else if (index == len)
                {
                    if (state == 0) { ActionPost = ""; break; }
                    else if (state == 1) ch = ' ';
                    else if (state == 2) break;
                    else if (state == 3) break;
                    else if (state == 4) ch = ' ';
                    else if (state == 5) { ch = '\\'; index--; }
                    else if (state == 6) { ch = '"'; index--; }
                    else if (state == 7) { ch = ' '; index--; }
                    else if (state == 8) { ch = '-'; }
                    else if (state == 9) { ch = ' '; }
                    else if (state == 10) { ch = ' '; }
                    else if (state == 11) { ch = '\\'; index--; }
                    else if (state == 12) { ch = '\\'; index--; }
                    else
                        break;
                }
                else
                    break;

                if (state == 0)
                {
                    if (ch.IsWhiteSpace()) { }
                    else if (ch.IsChar()) { valueBuilder.Append(ch); state = 1; }
                    else { errorflag = true; break; }
                }
                else if (state == 1)
                {
                    if (ch.IsWhiteSpace())
                    {
                        identity = valueBuilder.ToString(); valueBuilder.Clear(); state = 2;
                        if (index < len - 1) ActionPost = Raw.Substring(index + 1);
                        else ActionPost = "";
                    }
                    else if (ch.IsChar()) { valueBuilder.Append(ch); }
                    else { errorflag = true; break; }
                }
                else if (state == 2)
                {
                    if (ch == '-' || ch == '/') { state = 3; }
                    else if (ch.IsWhiteSpace()) { }
                    else if (ch.IsChar()) { valueBuilder.Append(ch); state = 4; }
                    else if (ch == '"') { state = 6; }
                    else if (ch == '\\') { state = 5; }
                    else { errorflag = true; break; }
                }
                else if (state == 3)
                {
                    if (ch.IsChar()) { valueBuilder.Append(ch); state = 7; }
                    else if (ch.IsWhiteSpace()) { state = 8; }
                    else { errorflag = true; break; }
                }
                else if (state == 4)
                {
                    if (ch.IsChar()) { valueBuilder.Append(ch); }
                    else if (ch == '\\') { state = 5; }
                    else if (ch == ';') { valueArray.Add(valueBuilder.ToString()); valueBuilder.Clear(); state = 9; }
                    else if (ch.IsWhiteSpace()) { action = valueBuilder.ToString(); namedargs["action"] = action; valueBuilder.Clear(); state = 8; }
                    else { errorflag = true; break; }
                }
                else if (state == 5)
                {
                    valueBuilder.Append(ch);
                    state = 4;
                }
                else if (state == 6)
                {
                    if (ch == '"') { valueArray.Add(valueBuilder.ToString()); valueBuilder.Clear(); state = 10; }
                    else if (ch == '\\') { state = 12; }
                    else { valueBuilder.Append(ch); }
                }
                else if (state == 7)
                {
                    if (ch.IsChar()) { valueBuilder.Append(ch); }
                    else if (ch.IsWhiteSpace()) { key = valueBuilder.ToString(); enterNamedArg = true; valueBuilder.Clear(); state = 8; }
                    else { errorflag = true; break; }
                }
                else if (state == 8)
                {
                    if (ch.IsWhiteSpace()) { }
                    else if (ch == '\\') { state = 11; }
                    else if (ch.IsChar()) { valueBuilder.Append(ch); state = 10; }
                    else if (ch == '-' || ch == '/')
                    {
                        if (enterNamedArg)
                        {
                            namedargs[key] = true;
                            enterNamedArg = false;
                        }
                        state = 3;
                    }
                    else { errorflag = true; break; }
                }
                else if (state == 9)
                {
                    if (ch.IsWhiteSpace())
                    {
                        if (valueArray.Count == 1) { unnamedargs.Add(valueArray[0]); valueArray.Clear(); }
                        else { unnamedargs.Add(valueArray.ToArray()); valueArray.Clear(); }
                        state = 8;
                    }
                    else if (ch == '"') { state = 6; }
                    else if (ch == ';') { }
                    else if (ch == '\\') { state = 11; }
                    else if (ch.IsChar()) { valueBuilder.Append(ch); state = 10; }
                    else { errorflag = true; break; }
                }
                else if (state == 10)
                {
                    if (ch == '\\') { state = 11; }
                    else if (ch.IsChar()) { valueBuilder.Append(ch); }
                    else if (ch == ';') { if (valueBuilder.Length > 0) { valueArray.Add(valueBuilder.ToString()); valueBuilder.Clear(); } state = 9; }
                    else if (ch.IsWhiteSpace())
                    {
                        if (valueBuilder.Length > 0)
                        {
                            valueArray.Add(valueBuilder.ToString());
                            valueBuilder.Clear();
                        }
                        if (enterNamedArg)
                        {

                            if (valueArray.Count == 1) { namedargs[key] = valueArray[0]; }
                            else { namedargs[key] = valueArray.ToArray(); }
                            enterNamedArg = false;
                        }
                        else
                        {
                            if (valueArray.Count == 1) { unnamedargs.Add(valueArray[0]); }
                            else { unnamedargs.Add(valueArray.ToArray()); }
                        }
                        valueArray.Clear();
                        state = 8;
                    }
                    else { errorflag = true; break; }
                }
                else if (state == 11)
                {
                    valueBuilder.Append(ch);
                    state = 10;
                }
                else if (state == 12)
                {
                    valueBuilder.Append(ch);
                    state = 6;
                }
                else
                    throw new Exception($"unknow state {state}");

                index++;
            }

            NamedArguments = namedargs;
            UnnamedArguments = unnamedargs;
            Identity = identity;
            Action = action;
            ContainsError = errorflag;
        }
    }
}
