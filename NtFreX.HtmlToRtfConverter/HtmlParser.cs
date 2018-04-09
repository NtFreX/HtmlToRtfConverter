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
                    var entity = Parse(enumerator);
                    if (entity != null)
                        yield return entity;
                }
            }
        }
        
        private static HtmlDomEntity Parse(IEnumerator<HtmlToken> enumerator)
        {
            if (enumerator.Current.TokenType == HtmlTokenType.CommentStart)
                return ParseComment(enumerator);
            if (enumerator.Current.TokenType == HtmlTokenType.ElementOpen)
                return ParseElement(enumerator);
            if (enumerator.Current.TokenType == HtmlTokenType.Text)
                return new HtmlText { Text = enumerator.Current.Value };
            return null;
        }
        private static HtmlDomEntity ParseElement(IEnumerator<HtmlToken> enumerator)
        {
            var isElementClosed = false;
            var element = new HtmlElement();
            while (enumerator.MoveNext() &&
                   enumerator.Current != null &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementFinish &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementInlineFinish)
            {
                if (string.IsNullOrEmpty(element.Name))
                    element.Name = ParseElementName(enumerator);
                else if (enumerator.Current.TokenType == HtmlTokenType.Attribute)
                    element.Attributes.AddRange(ParseAttribute(enumerator));
                else if (enumerator.Current.TokenType == HtmlTokenType.ElementClose)
                    isElementClosed = true;
                else if (isElementClosed)
                {
                    var entity = Parse(enumerator);
                    if (entity != null)
                        element.Children.Add(entity);
                }
            }

            if (enumerator.Current?.TokenType == HtmlTokenType.ElementFinish)
            {
                while (enumerator.Current.TokenType != HtmlTokenType.ElementClose)
                    enumerator.MoveNext();
            }

            return element;
        }
        private static HtmlComment ParseComment(IEnumerator<HtmlToken> enumerator)
        {
            var comment = new HtmlComment();
            while (enumerator.MoveNext() &&
                   enumerator.Current != null &&
                   enumerator.Current.TokenType != HtmlTokenType.CommentEnd)
            {
                if (enumerator.Current.TokenType == HtmlTokenType.Text)
                    comment.Value += enumerator.Current.Value;
            }
            return comment;
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
            var isAttributeValue = false;
            string currentAttributeName = null;

            while (enumerator.MoveNext() &&
                   enumerator.Current != null &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementClose &&
                   enumerator.Current.TokenType != HtmlTokenType.ElementInlineFinish)
            {
                if (!isAttributeValue && enumerator.Current.TokenType == HtmlTokenType.Text)
                    currentAttributeName = enumerator.Current.Value;
                else if (!isAttributeValue && enumerator.Current.TokenType == HtmlTokenType.AttributeValueSeperator)
                    isAttributeValue = true;
                else if (isAttributeValue && enumerator.Current.TokenType == HtmlTokenType.Text)
                {
                    yield return new HtmlAttribute
                    {
                        Name = currentAttributeName,
                        Value = enumerator.Current.Value
                    };
                }
                else if (isAttributeValue && enumerator.Current.TokenType == HtmlTokenType.AttributeValueFinish)
                    isAttributeValue = false;
            }
        }
    }
}