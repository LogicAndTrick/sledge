using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Common.Extensions
{
    public static class StringExtensions
    {
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
