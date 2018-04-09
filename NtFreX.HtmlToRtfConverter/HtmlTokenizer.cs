using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace NtFreX.HtmlToRtfConverter
{
    public class HtmlTokenizer : Tokenizer<
        HtmlToken,
        HtmlTokenMatch,
        HtmlTokenDefinition>
    {
        public HtmlTokenizer()
        {
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.CommentStart, @"^<!--"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.CommentStart, @"^-->"));

            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementFinish, @"^</"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementInlineFinish, @"^\/>"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementOpen, @"^<"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.ElementClose, @"^>"));

            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.Attribute, @"^="));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.AttributeValueSeperator, "^:"));
            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.AttributeValueFinish, "^;"));

            var value = string.Empty;
            foreach (var tokenDefinition in TokenDefinitions)
            {
                if (!string.IsNullOrEmpty(value))
                    value += "|";
                value += tokenDefinition.Regex.ToString().Substring(1);
            }

            TokenDefinitions.Add(new HtmlTokenDefinition(HtmlTokenType.Text, "^(((?!" + value + ").)*)"));
        }

        protected override HtmlToken Construct(HtmlTokenMatch match)
            => new HtmlToken(match.TokenType, match.Value);
    }
}