using System.Collections.Generic;

namespace NtFreX.HtmlToRtfConverter
{
    internal class RtfGeneratorSubject
    {
        public Dictionary<HtmlElementType, ElementSubject> ElementSubjects { get; set; } = new Dictionary<HtmlElementType, ElementSubject>();
        public string FontStyle { get; set; }
    }
}