using System.Collections.Generic;

namespace Sledge.Common.Extensions
{
    /// <summary>
    /// Common string extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Split a string by a delimiter without splitting sequences within quotes.
        /// </summary>
        /// <param name="line">The string to split</param>
        /// <param name="splitCharacters">The characters to split by. Defaults to space and tab characters if not specified.</param>
        /// <param name="quoteChar">The character which indicates the start or end of a quote</param>
        /// <returns>The split result, with split characters removed</returns>
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
