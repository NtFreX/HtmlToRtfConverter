using System.Collections.Generic;
using System.Linq;
using System.Text;
using NtFreX.HtmlToRtfConverter.Html.Dom;

namespace NtFreX.HtmlToRtfConverter
{
    public static class HtmlDomEntityExtensions
    {
        public static string GetHtml(this IEnumerable<HtmlDomEntity> entities)
            => GetHtmlInner(entities);
        public static string GetHtml(this HtmlDomEntity entity)
            => GetHtml(new[] { entity });

        private static string GetHtmlInner(this IEnumerable<HtmlDomEntity> entities, int tabCount = 0)
        {
            var value = new StringBuilder();
            var tabs = string.Join(string.Empty, Enumerable.Repeat("\t", tabCount));
            foreach (var entity in entities)
            {
                if (entity is HtmlComment comment)
                    value.AppendLine($"{tabs}<!--{comment.Value}-->");
                else if (entity is HtmlText text)
                    value.AppendLine($"{tabs}{text.Text}");
                else if (entity is HtmlElement element)
                {
                    var attribute = GetAttributeHtml(element);
                    value.AppendLine($"{tabs}<{element.Name} {attribute}>");
                    value.AppendLine(GetHtmlInner(element.Children, tabCount + 1));
                    value.AppendLine($"{tabs}</{element.Name}>");
                }
            }
            return value.ToString();
        }

        private static string GetAttributeHtml(HtmlElement element)
        {
            var value = new StringBuilder();
            foreach (var attribute in element.Attributes)
            {
                value.Append($"{attribute.Name}='{attribute.Value}' ");
            }
            return value.ToString();
        }
    }
}
