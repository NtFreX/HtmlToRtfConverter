using System.Collections.Generic;
using NtFreX.HtmlToRtfConverter.Html;
using NtFreX.HtmlToRtfConverter.Html.Dom;
using NtFreX.HtmlToRtfConverter.Rtf;

namespace NtFreX.HtmlToRtfConverter
{
    public class RtfConverter
    {
        private readonly RtfConverterSubject _subject;

        public RtfConverter()
            : this(new RtfConverterSubject()) { }
        internal RtfConverter(RtfConverterSubject subject)
            => _subject = subject;

        public string Convert(string html)
            => Convert(HtmlParser.Parse(html));
        public string Convert(HtmlDomEntity entity)
            => Convert(new[] {entity});
        public string Convert(IEnumerable<HtmlDomEntity> dom)
            => new RtfDocumentBuilder(_subject)
                .Html(dom)
                .Build();
    }
}