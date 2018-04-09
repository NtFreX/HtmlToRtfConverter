using System;
using System.Collections.Generic;
using System.Text;

namespace NtFreX.HtmlToRtfConverter
{
    public static class RtfGenerator
    {
        private const string DefaultFont = @"{\fonttbl {\f0 Times New Roman;}}";

        public static string Generate(string html)
        {
            var dom = HtmlParser.Parse(html);
            return GenerateInternal(dom);
        }

        private static string GenerateInternal(IEnumerable<HtmlDomEntity> dom)
        {
            var value = new StringBuilder();
            foreach (var obj in dom)
            {
                if (obj is HtmlElement element)
                {
                    value.AppendLine(@"{\rtf1\ansi\deff0 " + DefaultFont);
                    value.AppendLine(GenerateInternal(element.Children));
                    value.AppendLine("}");
                }
                else if (obj is HtmlText text)
                {
                    if (text.Text == "\r")
                        value.AppendLine(@"\line");
                    else
                        value.AppendLine(@"\f0\fs60 " + text.Text);
                }
            }

            return value.ToString();
        }
    }
}
