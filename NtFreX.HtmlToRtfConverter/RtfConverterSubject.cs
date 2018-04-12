using System.Collections.Generic;
using NtFreX.HtmlToRtfConverter.Html.Dom;

namespace NtFreX.HtmlToRtfConverter
{
    public class RtfConverterSubject
    {
        public Dictionary<HtmlElementType, ElementSubject> ElementSubjects = new Dictionary<HtmlElementType, ElementSubject>();
        public string FontStyle;
    }
}