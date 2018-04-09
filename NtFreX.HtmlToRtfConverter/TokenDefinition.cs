using System.Text.RegularExpressions;

namespace NtFreX.HtmlToRtfConverter
{
    public abstract class TokenDefinition<TTokenMatch> where TTokenMatch : TokenMatch, new()
    {
        private readonly Regex _regex;

        protected TokenDefinition(string regexPattern)
        {
            _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        }

        public virtual TTokenMatch Match(string inputString)
        {
            var match = _regex.Match(inputString);
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