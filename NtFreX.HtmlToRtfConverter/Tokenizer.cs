using System.Collections.Generic;

namespace NtFreX.HtmlToRtfConverter
{
    public abstract class Tokenizer<TToken, TTokenMatch, TTokenDefinition>
        where TTokenMatch : TokenMatch, new()
        where TTokenDefinition : TokenDefinition<TTokenMatch>
    {
        protected readonly List<TTokenDefinition> TokenDefinitions = new List<TTokenDefinition>();

        public IEnumerable<TToken> Tokenize(string text)
        {
            var remainingText = text;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);
                if (match.IsMatch)
                {
                    yield return Construct(match);
                    remainingText = match.RemainingText;
                }
                else
                {
                    remainingText = remainingText.Substring(1);
                }
            }
        }

        protected abstract TToken Construct(TTokenMatch match);

        private TTokenMatch FindMatch(string text)
        {
            foreach (var tokenDefinition in TokenDefinitions)
            {
                var match = tokenDefinition.Match(text);
                if (match.IsMatch)
                    return match;
            }

            return new TTokenMatch { IsMatch = false };
        }
    }
}