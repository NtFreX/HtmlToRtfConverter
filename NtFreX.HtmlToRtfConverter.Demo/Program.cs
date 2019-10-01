using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using NtFreX.HtmlToRtfConverter.Html;
using NtFreX.HtmlToRtfConverter.Html.Dom;
using NtFreX.HtmlToRtfConverter.Rtf;

namespace NtFreX.HtmlToRtfConverter.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
//            var html = @"
//<html>
//    <head>
//    </head>
//    <body>
//        <!-- this is just a comment -->
//        <div style='color:red;'>
//            <p />
//            Hello world
//        </div>
//        <p>This is a text</p>
//        <strong>This is a bold text</strong>
//        <em>Italic</em>
//        <del>Strikethrough</del>

//        <ul>
//            <li>Unorder list item</li>
//            <li>another item</li>
//        </ul>
//    </body>
//</html>";
            var html = @"
<p style='text-align: center; font-size: 12pt; font-family: Times New Roman'>center aligned</p>
<p style='text-align: left; font-size: 24pt; font-family: Arial'>left aligned</p>
<p style='text-align: right; font-size: 36pt; font-family: Helvetica'>right aligned</p>
<p style='text-align: justify; font-size: 48pt'>center aligned</p>

<div>
	<!--block-->
	Lorem ipsum dolor sit amet,
	<strong>consectetur</strong>
	adipiscing elit
	<del>Praesent lacus diam</del>
	, fermentum et venenatis quis, suscipit sed nisi. In pharetra sem eget orci posuere pretium.
	<em>Integer</em>
	non eros
	<strong>
		<em>scelerisque</em>
	</strong>
	, consequat lacus id, rutrum felis. Nulla elementum felis urna, at placerat arcu ultricies in.
</div>
<ul>
	<li>
		<!--block-->
		Proin elementum sollicitudin sodales.
	</li>
	<li>
		<!--block-->
		Nam id erat nec nibh dictum cursus.
	</li>
</ul>
<div>
	<!--block-->
	<br/>
</div>
<blockquote>
	<!--block-->
	<strong>asdasd</strong>
</blockquote>
<div>
	<!--block-->
	<br/>
</div>
<ol>
	<li>
		<!--block-->
		Proin elementum sollicitudin sodales.
	</li>
	<li>
		<!--block-->
		Nam id erat nec nibh dictum cursus.
	</li>
</ol>
<div>
	<!--block-->
	<br/>
</div>
<div>
	<!--block-->
	<br/>
</div>
<div>
	<!--block-->
	<br/>
</div>
<blockquote><!--block-->asd</blockquote>
<div>
	<!--block-->
	<br/>
</div>
<pre><!--block-->asdasd</pre>
<div style='background-color:green;'>
    Hello World
</div>
<blockquote>
	<!--block-->
	In et urna eros. Fusce molestie, orci vel laoreet tempus, sem justo blandit magna, at volutpat velit lacus id turpis.<br>Quisque malesuada sem at interdum congue. Aenean dapibus fermentum orci eu euismod.
</blockquote>
<div><!--block--><br></div>";

            var dom = HtmlParser
                .Parse(html)
                .ToArray();
            
            Console.WriteLine(dom.GetHtml());

            var converter = new RtfConverterBuilder()
                .Element(HtmlElementType.Pre, builder =>
                    builder.FontSize(12.0f)
                        .ForegroundColor(Color.Blue)
                        .BackgroundColor(Color.AliceBlue))
                .Element(HtmlElementType.Blockquote, builder =>
                    builder.FontSize(8f)
                        .ForegroundColor(Color.Black)
                        .BackgroundColor(Color.LightSlateGray)
                        .HorizontalAligment(HorizontalAlignment.Center))
                .BuildConverter();
            
            var value = converter.Convert(dom);

            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".rtf");
            File.WriteAllText(path, value);

            new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    UseShellExecute = true
                }
            }.Start();
        }
    }
}
