using System.Collections.Generic;

namespace NtFreX.HtmlToRtfConverter
{
    public class HtmlParser
    {
        public static IEnumerable<HtmlDomEntity> Parse(string html)
        {
            var tokenizer = new HtmlTokenizer();
            var tokens = tokenizer.Tokenize(html);

            using (var enumerator = tokens.GetEnumerator())
            {
                while (enumerator.MoveNext() && enumerator.Current != null)
                {
                    if (enumerator.Current.TokenType == HtmlTokenType.ElementOpen)
                    {
                        yield return ParseElement(enumerator);
                    }
                }
            }
        }

        private static HtmlDomEntity ParseElement(IEnumerator<HtmlToken> enumerator)
        {
            var state = 1;
            var element = new HtmlElement();
            while (enumerator.MoveNext() &&
                   enumerator.Current != null &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementFinish &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementInlineFinish)
            {
                if (string.IsNullOrEmpty(element.Name))
                    element.Name = ParseElementName(enumerator);
                else if (enumerator.Current.TokenType == HtmlTokenType.Attribute)
                {
                    state = 0;
                    element.Attributes.AddRange(ParseAttribute(enumerator));
                }
                else if (enumerator.Current.TokenType == HtmlTokenType.ElementOpen)
                    element.Children.Add(ParseElement(enumerator));
                else if (state == 0 && (enumerator.Current.TokenType == HtmlTokenType.Text || enumerator.Current.TokenType == HtmlTokenType.Spacing))
                    element.Children.Add(new HtmlText { Text = enumerator.Current.Value });
            }
            return element;
        }
        private static string ParseElementName(IEnumerator<HtmlToken> enumerator)
        {
            while (enumerator.Current != null && enumerator.Current.TokenType != HtmlTokenType.Text)
            {
                if (!enumerator.MoveNext())
                    return null;
            }

            return enumerator.Current?.Value;
        }
        private static IEnumerable<HtmlAttribute> ParseAttribute(IEnumerator<HtmlToken> enumerator)
        {
            var state = 0;
            var currentAttribute = new HtmlAttribute();

            while (enumerator.MoveNext() &&
                   enumerator.Current != null &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementClose &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementInlineFinish)
            {
                if (state == 0 && enumerator.Current.TokenType == HtmlTokenType.Text)
                    currentAttribute.Name = enumerator.Current.Value;
                else if (state == 0 && enumerator.Current.TokenType == HtmlTokenType.AttributeValueSeperator)
                    state = 1;
                else if (state == 1 && enumerator.Current.TokenType == HtmlTokenType.Text)
                {
                    currentAttribute.Value = enumerator.Current.Value;
                    yield return currentAttribute;
                    currentAttribute = new HtmlAttribute();
                }
                else if (state == 1 && enumerator.Current.TokenType == HtmlTokenType.AttributeValueFinish)
                    state = 0;
            }
        }
    }
}