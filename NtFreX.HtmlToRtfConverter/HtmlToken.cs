namespace NtFreX.HtmlToRtfConverter
{
    public class HtmlToken
    {
        public HtmlToken(HtmlTokenType tokenType)
        {
            TokenType = tokenType;
            Value = string.Empty;
        }

        public HtmlToken(HtmlTokenType tokenType, string value)
        {
            TokenType = tokenType;
            Value = value;
        }

        public HtmlTokenType TokenType { get; set; }
        public string Value { get; set; }

        public HtmlToken Clone()
        {
            return new HtmlToken(TokenType, Value);
        }
    }
}
