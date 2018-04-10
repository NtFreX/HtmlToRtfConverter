using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace NtFreX.HtmlToRtfConverter
{
    //TODO: lists (ordered, unordered); paragraphs
    public class RtfGenerator
    {
        private const string StyleAttributeName = "style";

        private const string ColorStyleName = "color";
        private const string BackgrounColorStyleName = "background-color";
        
        private const string StrongElementName = "strong";
        private const string EmElementName = "em";
        private const string DelElementName = "del";
        private const string LiElementName = "li";
        private const string OlElementName = "ol";
        private const string UlElementName = "ul";
        private const string PElementName = "p";
        private const string DivElementName = "div";
        private const string BrElementName = "br";
        private const string BlockQuoteElementName = "blockquote";
        private const string PreElementName = "pre";

        private readonly string[] _clearingElements =
        {
            PElementName,
            DivElementName,
            BlockQuoteElementName,
            PreElementName,
            LiElementName,
            OlElementName,
            UlElementName
        };
        
        private int _listLevel = -1;
        private bool _isOrderedList;

        private List<Color> _colors; 

        public string Generate(string html)
        {
            _colors = new List<Color>();

            var dom = HtmlParser.Parse(html);
            var rtf = GenerateInternal(dom, true);

            var value = new StringBuilder();
            value.AppendLine(@"{\rtf1\ansi\deff0 " + GenerateColorTable());
            value.AppendLine(rtf);
            value.AppendLine("}");
            return value.ToString();
        }

        private string GenerateInternal(IEnumerable<HtmlDomEntity> dom, bool isCleared)
        {
            var value = new StringBuilder();
            foreach (var obj in dom)
            {
                if (obj is HtmlElement element)
                {
                    if (element.Name.ToLower() == LiElementName)
                        _listLevel++;

                    var openingModifiers = string.Join(string.Empty, GetElementOpeningModifiers(element, isCleared));
                    var closingModifiers = string.Join(string.Empty, GetElementClosingModifiers(element));
                    var attributeModifiers = string.Join(string.Empty, GetAttributeModifiers(element));

                    value.AppendLine("{" + openingModifiers + attributeModifiers +
                                        GenerateInternal(element.Children, false) + closingModifiers + @"}");

                    if (element.Name.ToLower() == LiElementName)
                        _listLevel--;

                    isCleared = true;
                }
                else if (obj is HtmlText text)
                {
                    var textValue = RemoveSpacing(text.Text);
                    if(!string.IsNullOrWhiteSpace(textValue))
                        value.AppendLine("{" + textValue + "}");
                }
            }

            return value.ToString();
        }

        private string RemoveSpacing(string text)
        {
            text = text
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");

            while(text.Contains("  "))
                text = text.Replace("  ", " ");
            while (text.StartsWith(" "))
                text = text.Substring(1);
            while (text.EndsWith(" "))
                text = text.Substring(0, text.Length - 1);

            return text;
        }
        private IEnumerable<string> GetElementClosingModifiers(HtmlElement element)
        {
            var name = element.Name.ToLower();

            if (_clearingElements.Contains(name))
                yield return @"\par";
        }
        private IEnumerable<string> GetElementOpeningModifiers(HtmlElement element, bool isCleared)
        {
            var name = element.Name.ToLower();
            
            if (!isCleared && _clearingElements.Contains(name))
                yield return @"\par";
            if (name == StrongElementName)
                yield return @"\b";
            if (name == EmElementName)
                yield return @"\i";
            if (name == DelElementName)
                yield return @"\strike";
            if (name == OlElementName)
                _isOrderedList = true;
            if (name == UlElementName)
                _isOrderedList = false;
            if (name == LiElementName)
            {
                var indent = _listLevel * 400;
                var symbolType = _isOrderedList ? @"\pnlvl3" : @"\pnlvlblt\pntxtb\u8226?\tab";
                yield return @"{\li" + indent + @"\pntext\pn" + symbolType + @"}";
            }
            if (name == BlockQuoteElementName)
                yield return $@"\cf{GetColorNumber(Color.LightSlateGray)}\qc";
            if (name == PreElementName)
                yield return @"\brdrt\brdrs\brdrb\brdrs\brdrl\brdrs\brdrr\brdrs\brdrw10\brsp20\brdrcf0";
            if (name == BrElementName)
                yield return @"\line";
        }
        private IEnumerable<string> GetAttributeModifiers(HtmlElement element)
        {
            var style = element.Attributes.FirstOrDefault(x => x.Name.ToLower() == StyleAttributeName);
            if (style != null)
            {
                var styleParts = style.Value.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var stylePart in styleParts)
                {
                    var keyValue = stylePart.Split(":".ToCharArray());
                    var key = keyValue[0].ToLower();
                    var value = keyValue[1];
                    if (key == ColorStyleName)
                        yield return @"\cf" + GetColorNumber(HtmlColorHelper.GetColor(value));
                    if (key == BackgrounColorStyleName)
                    {
                        var colorNumber = GetColorNumber(HtmlColorHelper.GetColor(value));
                        yield return $@"\chshdng10000\chcbpat{colorNumber}\chcfpat{colorNumber}\cb{colorNumber}";
                    }
                }
            }
        }
        private int GetColorNumber(Color color)
        {
            var index = _colors.FindIndex(x => x.A == color.A && x.B == color.B && x.G == color.G && x.R == color.R);
            if (index >= 0)
            {
                return index + 1;
            }

            _colors.Add(color);
            return _colors.Count;
        }
        private string GenerateColorTable()
        {
            var value = new StringBuilder();
            value.Append(@"{\colortbl;");
            foreach(var color in _colors)
                value.Append(@"\red" + color.R + @"\green" + color.G + @"\blue" + color.B + @";");
            value.Append("}");
            return value.ToString();
        }
    }
}
