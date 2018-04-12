using System;
using System.Drawing;
using System.Linq;
using NtFreX.HtmlToRtfConverter.Html.Dom;
using NtFreX.HtmlToRtfConverter.Rtf;

namespace NtFreX.HtmlToRtfConverter
{
    internal static class RtfDocumentBuilderExtensions
    {
        private const string StyleAttributeName = "style";

        private const string ColorStyleName = "color";
        private const string BackgrounColorStyleName = "background-color";

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

        public static RtfDocumentBuilder ApplyConfigurationModifiers(this RtfDocumentBuilder builder, RtfConverterSubject subject, HtmlElementType elementType)
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

            builder.HorizontalAligment(configuration.HorizontalAligment);

            return builder;
        }
        public static RtfDocumentBuilder ApplyElementClosingModifiers(this RtfDocumentBuilder builder, HtmlElementType elementType)
        {
            if (ClearingElements.Contains(elementType))
                builder.Paragraph();
            return builder;
        }
        public static RtfDocumentBuilder ApplyElementOpeningModifiers(this RtfDocumentBuilder builder, HtmlElementType elementType, bool isCleared)
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
                builder.ForegroundColor(Color.LightSlateGray)
                    .HorizontalAligment(HorizontalAligment.Center);
            if (elementType == HtmlElementType.Pre)
                builder.Rtf(@"\brdrt\brdrs\brdrb\brdrs\brdrl\brdrs\brdrr\brdrs\brdrw10\brsp20\brdrcf0"); //TODO: move to rtfDocumentBuilder
            if (elementType == HtmlElementType.Br)
                builder.NewLine();

            return builder;
        }
        public static RtfDocumentBuilder ApplyAttributeModifiers(this RtfDocumentBuilder builder, HtmlElement element)
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
                    if (key == ColorStyleName)
                        builder.ForegroundColor(value);
                    else if (key == BackgrounColorStyleName)
                        builder.BackgroundColor(value);
                }
            }
            return builder;
        }
    }
}