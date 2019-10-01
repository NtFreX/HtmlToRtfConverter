using System;
using System.Globalization;

namespace NtFreX.HtmlToRtfConverter.Css
{
    internal class CssSize
    {
        public CssSizeType Type { get; set; }
        public float Value { get; set; }

        public CssSize(string value)
        {
            var normalized = value.Trim().ToLowerInvariant();

            if (normalized.EndsWith("pt")) Type = CssSizeType.Pt;
            else if (normalized.EndsWith("in")) Type = CssSizeType.In;
            else if (normalized.EndsWith("pc")) Type = CssSizeType.Pc;
            else if (normalized.EndsWith("px")) Type = CssSizeType.Px;
            else throw new NotSupportedException();

            Value = float.Parse(normalized.Substring(0, normalized.Length - 2));
        }

        public float ToPoints()
        {
            switch (Type)
            {
                case CssSizeType.Pt: return Value;
                case CssSizeType.In: return Value * 72f;
                case CssSizeType.Pc: return Value / 12;
                case CssSizeType.Px: return Value / 0.75f;
            }

            throw new NotSupportedException();
        }
    }
}