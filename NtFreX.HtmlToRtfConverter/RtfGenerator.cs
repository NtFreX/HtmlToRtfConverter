using System;
using System.Collections.Generic;
using System.Linq;
using NtFreX.HtmlToRtfConverter.Html;
using NtFreX.HtmlToRtfConverter.Html.Dom;
using NtFreX.HtmlToRtfConverter.Rtf;

namespace NtFreX.HtmlToRtfConverter
{
    //TODO: ¨multilevel lists (ordered, unordered); paragraphs
    public class RtfGenerator
    {
        private readonly RtfGeneratorSubject _subject;
        
        private RtfDocumentBuilder _documentBuilder;

        internal RtfGenerator(RtfGeneratorSubject subject)
        {
            _subject = subject;
        }

        public string Generate(string html)
        {
            _documentBuilder = new RtfDocumentBuilder(_subject);

            var dom = HtmlParser.Parse(html);
            GenerateInternal(dom, true);

            return _documentBuilder.Build();
        }

        private void GenerateInternal(IEnumerable<HtmlDomEntity> dom, bool isCleared)
        {
            foreach (var obj in dom)
            {
                if (obj is HtmlElement element)
                {
                    var name = new string(element.Name.Select((x, i) => i == 0 ? char.ToUpper(x) : char.ToLower(x)).ToArray());
                    if (!Enum.TryParse(name, out HtmlElementType elementType))
                        continue;

                    if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                        _documentBuilder.Context.ListLevel++;

                    _documentBuilder
                        .OpenContext()
                        .ApplyElementOpeningModifiers(elementType, isCleared)
                        .ApplyAttributeModifiers(element)
                            .OpenContext()
                            .ApplyConfigurationModifiers(_subject, elementType);

                    GenerateInternal(element.Children, false);

                    _documentBuilder
                        .CloseContext()
                        .ApplyElementClosingModifiers(elementType)
                        .CloseContext();

                    if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                        _documentBuilder.Context.ListLevel--;

                    isCleared = true;
                }
                else if (obj is HtmlText text)
                {
                    var textValue = RemoveSpacing(text.Text);
                    if (!string.IsNullOrWhiteSpace(textValue))
                        _documentBuilder.Text(textValue);                
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
