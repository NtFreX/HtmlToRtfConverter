namespace NtFreX.HtmlToRtfConverter
{
    public class HtmlTokenizer : Tokenizer<
        HtmlToken,
        HtmlTokenMatch,
        HtmlTokenDefinition>
    {

        public HtmlTokenizer()
        {
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.Spacing, @"^\s"));

            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementFinish, @"^</"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementInlineFinish, @"^\/>"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementOpen, @"^<"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementClose, @"^>"));

            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.Attribute, @"^="));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.AttributeValueSeperator, "^:"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.AttributeValueFinish, "^;"));

            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.Text, "^[a-zA-Z][a-zA-Z0-9]*"));
        }

        protected override HtmlToken Construct(HtmlTokenMatch match)
            => new HtmlToken(match.TokenType, match.Value);
    }
}