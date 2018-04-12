using System;
using NtFreX.HtmlToRtfConverter.Html.Dom;

namespace NtFreX.HtmlToRtfConverter
{
    public class RtfConverterBuilder : Builder<RtfConverterSubject>
    {
        public RtfConverterBuilder Element(HtmlElementType htmlElementType, Action<ElementBuilder> config)
            => InsertOrUpdate<RtfConverterBuilder, HtmlElementType, ElementSubject>(ref Subject.ElementSubjects, htmlElementType, Configure(config));
        public RtfConverterBuilder FontStyle(string fontStyle)
            => Apply<RtfConverterBuilder, string>(ref Subject.FontStyle, fontStyle);

        public RtfConverter BuildConverter()
            => new RtfConverter(Subject);

        private ElementSubject Configure(Action<ElementBuilder> config)
        {
            var elementBuilder = new ElementBuilder();
            config(elementBuilder);

            return elementBuilder.Build();
        }
    }
}
