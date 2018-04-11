using System.Collections.Generic;
using NtFreX.HtmlToRtfConverter.Html.Dom;

namespace NtFreX.HtmlToRtfConverter
{
    internal class RtfGeneratorSubject
    {
        public Dictionary<HtmlElementType, ElementSubject> ElementSubjects { get; set; } = new Dictionary<HtmlElementType, ElementSubject>();
        public string FontStyle { get; set; }
    }
}