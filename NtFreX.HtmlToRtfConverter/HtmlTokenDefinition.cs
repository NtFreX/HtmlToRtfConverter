namespace NtFreX.HtmlToRtfConverter
{
    public class HtmlTokenDefinition : TokenDefinition<HtmlTokenMatch>
    {
        private readonly HtmlTokenType _returnsToken;

        public HtmlTokenDefinition(HtmlTokenType returnsToken, string regexPattern)
            : base(regexPattern)
        {
            _returnsToken = returnsToken;
        }

        public override HtmlTokenMatch Match(string inputString)
        {
            var result = base.Match(inputString);
            if (result.IsMatch)
            {
                result.TokenType = _returnsToken;
            }
            return result;
        }
    }
}
