using System.Text.RegularExpressions;

namespace NtFreX.HtmlToRtfConverter.Tokenizer
{
    public abstract class TokenDefinition<TTokenMatch> where TTokenMatch : TokenMatch, new()
    {
        public Regex Regex { get; }

        protected TokenDefinition(string regexPattern)
        {
            Regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        }

        public virtual TTokenMatch Match(string inputString)
        {
            var match = Regex.Match(inputString);
            if (match.Success)
            {
                string remainingText = string.Empty;
                if (match.Length != inputString.Length)
                    remainingText = inputString.Substring(match.Length);

                return new TTokenMatch
                {
                    IsMatch = true,
                    RemainingText = remainingText,
                    Value = match.Value
                };
            }

            return new TTokenMatch { IsMatch = false };
        }
    }
}