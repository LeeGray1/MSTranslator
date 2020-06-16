using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using LanguageService;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        const string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=mstranslation;AccountKey=DhlfSrT66vg/I5CwpD0WrpeviWp5jrv/eyPaSTt7Pe8I0rv1PJnD3j8I7gGyc8oP0Jxs1+OpaL0U8Ku7kjFlFQ==;EndpointSuffix=core.windows.net";
        const string containerName = "xsltstorage";
        [TestMethod]
        public void TestUpdateTranslation()
        {
            string result = new LanguageClass(blobConnectionString, containerName).UpdateTranslation("delivery", "German","Evergreens");
            Assert.AreEqual(result.Substring(0, 5), "<?xml");
        }

        [TestMethod]
        public void TestDownloadAzure()
        {
            //string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            //BlobContainerClient container = new BlobContainerClient(connectionString, "xsltstorage");
            //var blockBlob = container.GetBlobClient("stylesheet-ubl v2.xslt");
            //System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //ms.Position = 0;
            //blockBlob.DownloadTo(ms);
            //string file = System.Text.Encoding.ASCII.GetString(ms.ToArray());           

            //Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestUploadFile2Blob()
        {
            string fileName = "cleaning services.xml";
            string filePath = @"..\..\..\MSTranslatordemo\";
            string file = File.ReadAllText(Path.Combine(filePath, fileName));
            
            new LanguageService.LanguageClass(blobConnectionString, containerName).UploadFileToBlob(file, fileName);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestSaxonTransform()
        {

            string xsltFilePath = @"..\..\..\MSTranslatordemo\stylesheet-ubl v2.xslt";
            string xmlFilePath = @"..\..\..\MSTranslatordemo\cleaning services.xml";
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

            Assert.AreEqual("<html", htmlstring.Substring(0, 5));


        }
        [TestMethod]
        public void TestDownLoadFileFromBlob()
        {
            string fileName = "Hindi-stylesheet-ubl.xslt";
            //string filePath = @"..\..\..\..\MSTranslatordemo\";
            //string file = File.ReadAllText(Path.Combine(filePath, fileName));
            string fileContents = new LanguageService.LanguageClass(blobConnectionString, containerName).DownloadFileFromBlob(fileName, blobConnectionString, containerName);
            Assert.IsTrue(fileContents.Substring(0, 5) == "<?xml");

        }

        [TestMethod]
        public void TestCreateTranslatedHtmlfromXML()
        {
            string OriginalFullPathName = @"..\..\..\MSTranslatordemo\cleaning services.xml";
            string ToLanguage = "german";

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            string containerName = "xsltstorage";
            string filePath = Path.GetFullPath(OriginalFullPathName);
            string OriginalxmlFile = File.ReadAllText(filePath);
            Task<string> task = new LanguageService.LanguageClass(blobConnectionString, containerName).ConvertXml2Html(OriginalxmlFile, ToLanguage, filePath, connectionString, containerName);
            task.Wait();
            string HTMLstring = task.Result;
            File.WriteAllText("eInvoice.html", HTMLstring);
            System.Diagnostics.Process.Start("eInvoice.html");

            Assert.AreEqual("<html", HTMLstring.Substring(0,5));

        }

        [TestMethod]
        public void TestDetectLanguage()
        {
            string result = new LanguageClass(blobConnectionString, containerName).DetectLanguage("Hallo");
            Assert.AreNotEqual(result, "");

        }
    }
}
