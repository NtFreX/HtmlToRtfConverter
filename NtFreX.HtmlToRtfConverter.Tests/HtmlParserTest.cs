using System.Linq;
using NtFreX.HtmlToRtfConverter.Html;
using NtFreX.HtmlToRtfConverter.Html.Dom;
using Xunit;

namespace NtFreX.HtmlToRtfConverter.Tests
{
    public class HtmlParserTest
    {
        [Fact]
        public void Can_Parse_VoidElement()
        {
            var html = "<div><!--block--><br></div>";

            var result = HtmlParser.Parse(html)
                .ToList();

            Assert.Single(result);
            Assert.True(result.First() is HtmlElement);

            var element = result.First() as HtmlElement;
            Assert.NotNull(element);
            Assert.Equal("div", element.Name);
            Assert.Equal(2, element.Children.Count);
            Assert.Empty(element.Attributes);

            Assert.True(element.Children.First() is HtmlComment);
            var comment = element.Children.First() as HtmlComment;
            Assert.NotNull(comment);
            Assert.Equal("block", comment.Value);

            Assert.True(element.Children.Last() is HtmlElement);
            var brEle = element.Children.Last() as HtmlElement;
            Assert.NotNull(brEle);
            Assert.Equal("br", brEle.Name);
            Assert.Empty(brEle.Attributes);
            Assert.Empty(brEle.Children);
        }

        [Fact]
        public void Can_Parse_InlineElementFinish()
        {
            var html = "<div><!--block--><br/></div>";

            var result = HtmlParser.Parse(html)
                .ToList();

            Assert.Single(result);
            Assert.True(result.First() is HtmlElement);

            var element = result.First() as HtmlElement;
            Assert.NotNull(element);
            Assert.Equal("div", element.Name);
            Assert.Equal(2, element.Children.Count);
            Assert.Empty(element.Attributes);

            Assert.True(element.Children.First() is HtmlComment);
            var comment = element.Children.First() as HtmlComment;
            Assert.NotNull(comment);
            Assert.Equal("block", comment.Value);

            Assert.True(element.Children.Last() is HtmlElement);
            var brEle = element.Children.Last() as HtmlElement;
            Assert.NotNull(brEle);
            Assert.Equal("br", brEle.Name);
            Assert.Empty(brEle.Attributes);
            Assert.Empty(brEle.Children);
        }

        [Theory]
        [InlineData("<{0}/>")]
        [InlineData("<{0}></{0}>")]
        public void Can_Parse_SingleElement(string html)
        {
            var elementName = "div";

            html = string.Format(html, elementName);

            var result = HtmlParser.Parse(html)
                                   .ToList();

            Assert.Single(result);
            Assert.True(result.First() is HtmlElement);

            var element = result.First() as HtmlElement;
            Assert.NotNull(element);
            Assert.Equal(element.Name, elementName);
            Assert.Empty(element.Children);
            Assert.Empty(element.Attributes);
        }

        [Theory]
        [InlineData("<{0} {1}=\"{2}\"/>")]
        [InlineData("<{0} {1}={2}'/>")]
        [InlineData("<{0} {1}=\"{2}\"></{0}>")]
        [InlineData("<{0} {1}='{2}'></{0}>")]
        public void Can_Parse_SingleAttribute(string html)
        {
            var elementName = "div";
            var attributeName = "style";
            var attributeValue = "color:red;";

            html = string.Format(html, elementName, attributeName, attributeValue);

            var result = HtmlParser.Parse(html)
                .ToList();

            Assert.Single(result);
            Assert.True(result.First() is HtmlElement);

            var element = result.First() as HtmlElement;
            Assert.NotNull(element);
            Assert.Equal(element.Name, elementName);
            Assert.Empty(element.Children);

            Assert.Single(element.Attributes);
            Assert.Equal(attributeName, element.Attributes.First().Name);
            Assert.Equal(attributeValue, element.Attributes.First().Value);
        }

        [Theory]
        [InlineData("Hey this is a comment")]
        [InlineData("¨p2ü43o2lääö")]
        [InlineData(" -- -- ")]
        public void Can_Parse_Comment(string comment)
        {
            var html = string.Format("<!--{0}-->", comment);

            var result = HtmlParser.Parse(html)
                .ToList();

            Assert.Single(result);
            Assert.True(result.First() is HtmlComment);

            var element = result.First() as HtmlComment;
            Assert.NotNull(element);
            Assert.Equal(element.Value, comment);
        }

        [Theory]
        [InlineData("<{0}><{1}/></{0}>")]
        [InlineData("<{0}><{1}></{1}></{0}>")]
        public void Can_Parse_SingleChild(string html)
        {
            var elementName = "div";
            var childElementName = "p";

            html = string.Format(html, elementName, childElementName);

            var result = HtmlParser.Parse(html)
                .ToList();

            Assert.Single(result);
            Assert.True(result.First() is HtmlElement);

            var element = result.First() as HtmlElement;
            Assert.NotNull(element);
            Assert.Equal(element.Name, elementName);
            Assert.Empty(element.Attributes);

            Assert.Single(element.Children);
            var childElement = element.Children.First() as HtmlElement;
            Assert.NotNull(childElement);
            Assert.Equal(childElement.Name, childElementName);
            Assert.Empty(childElement.Attributes);
            Assert.Empty(childElement.Children);
        }
    }
}
