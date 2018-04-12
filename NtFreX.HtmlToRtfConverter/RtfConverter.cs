using System;
using System.Collections.Generic;
using System.Linq;
using NtFreX.HtmlToRtfConverter.Html;
using NtFreX.HtmlToRtfConverter.Html.Dom;
using NtFreX.HtmlToRtfConverter.Rtf;

namespace NtFreX.HtmlToRtfConverter
{
    //TODO: ¨multilevel lists (ordered, unordered); background color; paragraphs
    public class RtfConverter
    {
        private readonly RtfConverterSubject _subject;

        internal RtfConverter(RtfConverterSubject subject)
        {
            _subject = subject;
        }

        public string Convert(string html)
        {
            var dom = HtmlParser.Parse(html);
            return Convert(dom);
        }
        public string Convert(HtmlDomEntity entity)
            => Convert(new[] {entity});
        public string Convert(IEnumerable<HtmlDomEntity> dom)
        {
            var documentBuilder = new RtfDocumentBuilder(_subject);

            ConvertInternal(dom, documentBuilder, true);

            return documentBuilder.Build();
        }

        private void ConvertInternal(IEnumerable<HtmlDomEntity> dom, RtfDocumentBuilder documentBuilder, bool isCleared)
        {
            foreach (var obj in dom)
            {
                if (obj is HtmlElement element)
                {
                    var name = new string(element.Name.Select((x, i) => i == 0 ? char.ToUpper(x) : char.ToLower(x)).ToArray());
                    if (!Enum.TryParse(name, out HtmlElementType elementType))
                        continue;

                    if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                        documentBuilder.Context.ListLevel++;

                    documentBuilder
                        .OpenContext()
                        .ApplyElementOpeningModifiers(elementType, isCleared)
                        .ApplyAttributeModifiers(element)
                            .OpenContext()
                            .ApplyConfigurationModifiers(_subject, elementType);

                    ConvertInternal(element.Children, documentBuilder, false);

                    documentBuilder
                        .CloseContext()
                        .ApplyElementClosingModifiers(elementType)
                        .CloseContext();

                    if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                        documentBuilder.Context.ListLevel--;

                    isCleared = true;
                }
                else if (obj is HtmlText text)
                {
                    var textValue = RemoveSpacing(text.Text);
                    if (!string.IsNullOrWhiteSpace(textValue))
                        documentBuilder.Text(textValue);                
                }
            }
        }

        private string RemoveSpacing(string text)
        {
            text = text
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");

            while(text.Contains("  "))
                text = text.Replace("  ", " ");
            while (text.StartsWith(" "))
                text = text.Substring(1);
            while (text.EndsWith(" "))
                text = text.Substring(0, text.Length - 1);

            return text;
        }
    }
}
