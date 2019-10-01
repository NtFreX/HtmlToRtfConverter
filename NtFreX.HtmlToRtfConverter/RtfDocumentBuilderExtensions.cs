using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NtFreX.HtmlToRtfConverter.Html.Dom;
using NtFreX.HtmlToRtfConverter.Rtf;

namespace NtFreX.HtmlToRtfConverter
{
    //TODO: ¨multilevel lists (ordered, unordered); background color; paragraphs
    internal static class RtfDocumentBuilderExtensions
    {
        private const string StyleAttributeName = "style";

        private static readonly HtmlElementType[] ClearingElements =
        {
            HtmlElementType.P,
            HtmlElementType.Div,
            HtmlElementType.Blockquote,
            HtmlElementType.Pre,
            HtmlElementType.Li,
            HtmlElementType.Ol,
            HtmlElementType.Ul
        };

        public static RtfDocumentBuilder Html(this RtfDocumentBuilder builder, IEnumerable<HtmlDomEntity> dom)
            => builder.HtmlInner(dom, true);
        private static RtfDocumentBuilder HtmlInner(this RtfDocumentBuilder builder, IEnumerable<HtmlDomEntity> dom, bool isCleared)
        {
            foreach (var obj in dom)
            {
                if (obj is HtmlElement element)
                {
                    var name = ToFirstLetterUpper(element.Name);
                    if (!Enum.TryParse(name, out HtmlElementType elementType))
                        continue;

                    if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                        builder.Context.ListLevel++;

                    builder
                        .OpenContext()
                        .ApplyElementOpeningModifiers(elementType, isCleared)
                        .ApplyAttributeModifiers(element)
                            .OpenContext()
                            .ApplyConfigurationModifiers(builder.Subject, elementType)
                            .HtmlInner(element.Children, false)
                            .CloseContext()
                        .ApplyElementClosingModifiers(elementType)
                        .CloseContext();

                    if (elementType == HtmlElementType.Ol || elementType == HtmlElementType.Ul)
                        builder.Context.ListLevel--;

                    isCleared = true;
                }
                else if (obj is HtmlText text)
                {
                    var textValue = RemoveSpacing(text.Text);
                    if (!string.IsNullOrWhiteSpace(textValue))
                        builder.Text(textValue);
                }
            }
            return builder;
        }
        private static RtfDocumentBuilder ApplyConfigurationModifiers(this RtfDocumentBuilder builder, RtfConverterSubject subject, HtmlElementType elementType)
        {
            if (!subject.ElementSubjects.ContainsKey(elementType))
                return builder;

            var configuration = subject.ElementSubjects[elementType];

            if (!configuration.ForegroundColor.IsEmpty)
                builder.ForegroundColor(configuration.ForegroundColor);
            if (!configuration.BackgroundColor.IsEmpty)
                builder.BackgroundColor(configuration.BackgroundColor);
            if (Math.Abs(configuration.FontSize - default(float)) < 0)
                builder.FontSize(configuration.FontSize);
            if (!string.IsNullOrEmpty(configuration.FontStyle))
                builder.FontStyle(configuration.FontStyle);

            builder.HorizontalAlignment(configuration.HorizontalAlignment);

            return builder;
        }
        private static RtfDocumentBuilder ApplyElementClosingModifiers(this RtfDocumentBuilder builder, HtmlElementType elementType)
        {
            if (ClearingElements.Contains(elementType))
                builder.Paragraph();
            return builder;
        }
        private static RtfDocumentBuilder ApplyElementOpeningModifiers(this RtfDocumentBuilder builder, HtmlElementType elementType, bool isCleared)
        {
            if (!isCleared && ClearingElements.Contains(elementType))
                builder.Paragraph();
            if (elementType == HtmlElementType.Strong)
                builder.Bold();
            if (elementType == HtmlElementType.Em)
                builder.Italic();
            if (elementType == HtmlElementType.Del)
                builder.Striketrough();
            if (elementType == HtmlElementType.Ol)
                builder.Context.IsOrderedList = true;
            if (elementType == HtmlElementType.Ul)
                builder.Context.IsOrderedList = false;
            if (elementType == HtmlElementType.Li)
            {
                var indent = builder.Context.ListLevel * 400;
                var symbolType = builder.Context.IsOrderedList ? @"\pnlvl3" : @"\pnlvlblt\pntxtb\u8226?\tab";
                builder.Rtf(@"{\li" + indent + @"\pntext\pn" + symbolType + @"}"); //TODO: move to rtfDocumentBuilder
            }
            if (elementType == HtmlElementType.Blockquote)
                builder
                    .ForegroundColor(Color.LightSlateGray)
                    .HorizontalAlignment(HorizontalAlignment.Center);
            if (elementType == HtmlElementType.Pre)
                builder.Rtf(@"\brdrt\brdrs\brdrb\brdrs\brdrl\brdrs\brdrr\brdrs\brdrw10\brsp20\brdrcf0"); //TODO: move to rtfDocumentBuilder
            if (elementType == HtmlElementType.Br)
                builder.NewLine();

            return builder;
        }
        private static RtfDocumentBuilder ApplyAttributeModifiers(this RtfDocumentBuilder builder, HtmlElement element)
        {
            var style = element.Attributes.FirstOrDefault(x => x.Name.ToLower() == StyleAttributeName);
            if (style != null)
            {
                var styleParts = style.Value.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var stylePart in styleParts)
                {
                    var keyValue = stylePart.Split(":".ToCharArray());
                    var key = keyValue[0].ToLower();
                    var value = keyValue[1];

                    HtmlAttributeModifiers.ApplyAttribute(key, value, builder);
                }
            }
            return builder;
        }

        private static string ToFirstLetterUpper(string value)
            => new string(value.Select((x, i) => i == 0 ? char.ToUpper(x) : char.ToLower(x)).ToArray());
        private static string RemoveSpacing(string text)
        {
            text = text
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");

            while (text.Contains("  "))
                text = text.Replace("  ", " ");
            while (text.StartsWith(" "))
                text = text.Substring(1);
            while (text.EndsWith(" "))
                text = text.Substring(0, text.Length - 1);

            return text;
        }
    }
}