using System;
using NtFreX.HtmlToRtfConverter.Html.Dom;

namespace NtFreX.HtmlToRtfConverter
{
    public class RtfGeneratorBuilder
    {
        private readonly RtfGeneratorSubject _subject = new RtfGeneratorSubject();

        public RtfGeneratorBuilder Element(HtmlElementType htmlElementType, Action<ElementBuilder> config)
        {
            var elementBuilder = new ElementBuilder();
            config(elementBuilder);

            var elementSubject = elementBuilder.Build();
            if (_subject.ElementSubjects.ContainsKey(htmlElementType))
                _subject.ElementSubjects[htmlElementType] = elementSubject;
            else
                _subject.ElementSubjects.Add(htmlElementType, elementSubject);

            return this;
        }

        public RtfGeneratorBuilder FontStyle(string fontStyle)
        {
            _subject.FontStyle = fontStyle;
            return this;
        }

        public RtfGenerator Build()
            => new RtfGenerator(_subject);
    }
}
