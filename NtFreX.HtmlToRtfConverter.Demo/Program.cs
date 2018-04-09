using System;
using System.Diagnostics;
using System.IO;

namespace NtFreX.HtmlToRtfConverter.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var value = RtfGenerator.Generate(@"
<html>
    <head>
    </head>
    <body>
        <div style='color:red;'>
            <p />
            Hello world
        </div>
        <p>This is a text</p>
    </body>
</html>");

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
