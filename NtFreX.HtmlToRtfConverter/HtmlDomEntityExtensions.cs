using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    value.AppendLine($"{tabs}<{element.Name}>");
                    value.AppendLine(GetHtmlInner(element.Children, tabCount + 1));
                    value.AppendLine($"{tabs}</{element.Name}>");
                }
            }
            return value.ToString();
        }
    }
}
