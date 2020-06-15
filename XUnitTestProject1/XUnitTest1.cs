using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTestProject1
{
    public class XUnitTest1
    {
        [Fact]
        public void TestSaxonTransform()
        {

            string xsltFilePath = @"..\..\..\..\MSTranslatordemo\stylesheet-ubl.xslt";
            string xmlFilePath = @"..\..\..\..\MSTranslatordemo\cleaning services.xml";
            MemoryStream xsltStream = new MemoryStream();
            using (FileStream fileStream = File.OpenRead(xsltFilePath))
            {
                
                xsltStream.SetLength(fileStream.Length);
                fileStream.Read(xsltStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            MemoryStream xmlStream = new MemoryStream();
            using (FileStream fileStream = File.OpenRead(xmlFilePath))
            {

                xmlStream.SetLength(fileStream.Length);
                fileStream.Read(xmlStream.GetBuffer(), 0, (int)fileStream.Length);
            }
            //string htmlstring = LanguageService.XSLThelper.SaxonTransform(xsltFilePath, xmlFilePath);
            string htmlstring = new XSLTLibrary.SaxonUtils().Transform(xmlStream, xsltStream);

            Assert.Equal( "<html", htmlstring.Substring(0, 5));


        }

        [Fact]
        public void  TestUploadFileToBlob()
        {
            string fileName = "Hindi-stylesheet-ubl.xslt";
            string filePath = @"..\..\..\..\MSTranslatordemo\";
            string file = File.ReadAllText(Path.Combine(filePath, fileName));
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            string containerName = "xsltstorage";
            new LanguageService.LanguageClass().UploadFileToBlob(file, fileName, connectionString, containerName);
            Assert.True(true);

        }

        [Fact]
        public void TestDownLoadFileFromBlob()
        {
            string fileName = "Hindi-stylesheet-ubl.xslt";
            //string filePath = @"..\..\..\..\MSTranslatordemo\";
            //string file = File.ReadAllText(Path.Combine(filePath, fileName));
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            string containerName = "xsltstorage";
            string fileContents = new LanguageService.LanguageClass().DownloadFileFromBlob( fileName, connectionString, containerName);
            Assert.True(fileContents.Substring(0,5)== "<?xml");

        }

        [Theory]
        [InlineData(@"..\..\..\..\MSTranslatordemo\cleaning services.xml", "french")]
        public void TestCreateTranslatedHtmlfromXML(string OriginalFullPathName, string ToLanguage)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            string containerName = "xsltstorage";
            string filePath = Path.GetFullPath(OriginalFullPathName);
            string OriginalxmlFile = File.ReadAllText(filePath);
            Task<string> task = new LanguageService.LanguageClass().ConvertXml2Html(OriginalxmlFile, ToLanguage, filePath,connectionString,containerName);
            task.Wait();
            string res = task.Result;
            Assert.Equal("", res);
            
        }
    }
}
