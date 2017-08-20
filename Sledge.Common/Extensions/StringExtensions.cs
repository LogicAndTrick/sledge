using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Split a string, but don't split within quoted values.
        /// </summary>
        /// <param name="line">The line to split</param>
        /// <param name="splitTest">Optional split test. Defaults to whitespace test.</param>
        /// <param name="quoteChar">Optional quote character. Defaults to double quote.</param>
        /// <returns>An array of split values</returns>
        public static string[] SplitWithQuotes2(this string line, Func<char, bool> splitTest = null, char quoteChar = '"')
        {
            if (splitTest == null) splitTest = Char.IsWhiteSpace;
            var result = new List<string>();
            var index = 0;
            var inQuote = false;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                var isSplitter = splitTest(c);
                if (isSplitter && index == i)
                {
                    index = i + 1;
                }
                else if (c == quoteChar)
                {
                    inQuote = !inQuote;
                }
                else if (isSplitter && !inQuote)
                {
                    result.Add(line.Substring(index, i - index).Trim(quoteChar));
                    index = i + 1;
                }
                if (i != line.Length - 1) continue;
                result.Add(line.Substring(index, (i + 1) - index).Trim(quoteChar));
            }
            return result.ToArray();
        }

        public static string[] SplitWithQuotes(this string line, char[] splitCharacters = null, char quoteChar = '"')
        {
            if (splitCharacters == null) splitCharacters = new[] { ' ', '\t' };

            var result = new List<string>();

            int i;
            for (i = 0; i < line.Length; i++)
            {
                var split = line.IndexOfAny(splitCharacters, i);
                var quote = line.IndexOf(quoteChar, i);

                if (split < 0) split = line.Length;
                if (quote < 0) quote = line.Length;

                if (quote < split)
                {
                    if (quote > i) result.Add(line.Substring(i, quote));
                    var nextQuote = line.IndexOf(quoteChar, quote + 1);
                    if (nextQuote < 0) nextQuote = line.Length;
                    result.Add(line.Substring(quote + 1, nextQuote - quote - 1));
                    i = nextQuote;
                }
                else
                {
                    if (split > i) result.Add(line.Substring(i, split - i));
                    i = split;
                }
            }
            return result.ToArray();
        }
    }
}
