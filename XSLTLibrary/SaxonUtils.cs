using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XSLTLibrary
{
    public class SaxonUtils
    {
        public string Transform(Stream XMLStream, Stream xsltStream)
        {

            // Compile stylesheet

            var processor = new Saxon.Api.Processor();
            var compiler = processor.NewXsltCompiler();
            var executable = compiler.Compile(xsltStream);

            // Do transformation to a destination
            var destination = new Saxon.Api.DomDestination();
            // using (var inputStream = input.OpenRead())
            {
                var transformer = executable.Load();
                transformer.SetInputStream(XMLStream, new Uri("http://www.w3.org/"));
                transformer.Run(destination);
            }

            // Save result to a file (or whatever else you wanna do)
            // destination.XmlDocument.Save(output.FullName);
            return destination.XmlDocument.OuterXml;

        }
    }
}
