﻿using System.Drawing;
using System.Text;
using NtFreX.HtmlToRtfConverter.Html;

namespace NtFreX.HtmlToRtfConverter.Rtf
{
    internal class RtfDocumentBuilder
    {
        private readonly StringBuilder _document = new StringBuilder();

        public RtfConverterSubject Subject { get; }
        public RtfDocumentContext Context { get; } = new RtfDocumentContext();

        public RtfDocumentBuilder(RtfConverterSubject subject)
            => Subject = subject;

        public RtfDocumentBuilder BackgroundColor(string htmlColor) => BackgroundColor(HtmlColorHelper.GetColor(htmlColor));
        public RtfDocumentBuilder BackgroundColor(Color color)
        {
            var colorNumber = Context.GetColorNumber(color);
            return Rtf($@"\chshdng10000\chcbpat{colorNumber}\chcfpat{colorNumber}\cb{colorNumber}");
        }
        public RtfDocumentBuilder HorizontalAlignment(HorizontalAlignment alignment)
        {
            if (alignment == HtmlToRtfConverter.Rtf.HorizontalAlignment.Left)
                _document.Append(@"\ql");
            else if (alignment == HtmlToRtfConverter.Rtf.HorizontalAlignment.Right)
                _document.Append(@"\qr");
            else if (alignment == HtmlToRtfConverter.Rtf.HorizontalAlignment.Center)
                _document.Append(@"\qc");
            else if (alignment == HtmlToRtfConverter.Rtf.HorizontalAlignment.Justify)
                _document.Append(@"\qj");
            return this;
        }
        public RtfDocumentBuilder ForegroundColor(string htmlColor) => ForegroundColor(HtmlColorHelper.GetColor(htmlColor));
        public RtfDocumentBuilder ForegroundColor(Color color) => Rtf($@"\cf{Context.GetColorNumber(color)}");
        public RtfDocumentBuilder Paragraph() => Rtf(@"\par");
        public RtfDocumentBuilder FontSize(float fontSize) => Rtf($@"\fs{fontSize}");
        public RtfDocumentBuilder FontStyle(string font) => Rtf($@"\f{Context.GetFontNumber(font)}");
        public RtfDocumentBuilder Bold() => Rtf(@"\b");
        public RtfDocumentBuilder Italic() => Rtf(@"\i");
        public RtfDocumentBuilder Striketrough() => Rtf(@"\strike");
        public RtfDocumentBuilder NewLine() => Rtf(@"\line");
        public RtfDocumentBuilder Text(string text) => Rtf("{" + text + "}");
        public RtfDocumentBuilder OpenContext() => Rtf("{");
        public RtfDocumentBuilder CloseContext() => Rtf("}");
        public RtfDocumentBuilder Rtf(string rtf)
        {
            _document.Append(rtf);
            return this;
        }

        public string Build()
        {
            var fontTable = Context.GetFontTable(Subject.FontStyle);
            var colorTable = Context.GetColorTable();

            var value = new StringBuilder();
            value.AppendLine(@"{\rtf1\ansi\deff0 " + fontTable + colorTable);
            value.AppendLine(_document.ToString());
            value.AppendLine("}");
            return value.ToString();
        }
    }
}