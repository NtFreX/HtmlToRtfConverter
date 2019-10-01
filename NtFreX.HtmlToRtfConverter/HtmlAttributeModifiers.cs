using System;
using System.Collections.Generic;
using NtFreX.HtmlToRtfConverter.Rtf;

namespace NtFreX.HtmlToRtfConverter
{
    internal static class HtmlAttributeModifiers
    {
        private const string Color = "color";
        private const string BackgroundColor = "background-color";
        private const string TextAlign = "text-align";
        private const string FontSize = "font-size";
        private const string FontFamily = "font-family";

        private static readonly Dictionary<string, Action<RtfDocumentBuilder, string>> AttributeModifiers = new Dictionary<string, Action<RtfDocumentBuilder, string>>
        {
            { Color, (builder, value) => builder.ForegroundColor(value) },
            { BackgroundColor, (builder, value) => builder.BackgroundColor(value) },
            { TextAlign, (builder, value) =>
                {
                    if (Enum.TryParse(value.Trim().FirstCharToUpper(), out HorizontalAlignment horizontalAlignment))
                    {
                        builder.HorizontalAlignment(horizontalAlignment);
                    }
                }
            },
            { FontSize, (builder, value) => builder.FontSize(float.Parse(value)) },
            { FontFamily, (builder, value) => builder.FontFamily(value) }
        };

        public static void ApplyAttribute(string name, string value, RtfDocumentBuilder builder)
        {
            if (AttributeModifiers.ContainsKey(name))
            {
                AttributeModifiers[name].Invoke(builder, value);
            }
        }
    }
}