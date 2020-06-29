using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using LanguageService;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        const string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=mstranslation;AccountKey=DhlfSrT66vg/I5CwpD0WrpeviWp5jrv/eyPaSTt7Pe8I0rv1PJnD3j8I7gGyc8oP0Jxs1+OpaL0U8Ku7kjFlFQ==;EndpointSuffix=core.windows.net";
        const string containerName = "xsltstorage";
        const string baseUri = "https://localhost:44330";
        [TestMethod]
        public void TestUpdateTranslation()
        {
            string result = new LanguageClass(blobConnectionString, containerName).UpdateTranslation("delivery", "German","Evergreens");
            Assert.AreEqual(result.Substring(0, 5), "<?xml");
        }

        //[TestMethod]
        //public void TestDownloadAzure()
        //{
        //    //string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
        //    //BlobContainerClient container = new BlobContainerClient(connectionString, "xsltstorage");
        //    //var blockBlob = container.GetBlobClient("stylesheet-ubl v2.xslt");
        //    //System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //    //ms.Position = 0;
        //    //blockBlob.DownloadTo(ms);
        //    //string file = System.Text.Encoding.ASCII.GetString(ms.ToArray());           

        //    //Assert.IsTrue(true);
        //}

        [TestMethod]
        public void TestUploadFile2Blob()
        {
            string fileName = "Gujarati-cleaning services.xml";
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
            string fileContents = new LanguageService.LanguageClass(blobConnectionString, containerName).DownloadFileFromBlob(fileName);
            Assert.IsTrue(fileContents.Substring(0, 5) == "<?xml");

        }

        [TestMethod]
        public void TestCreateTranslatedHtmlfromXML()
        {
            string OriginalFullPathName = @"..\..\..\MSTranslatordemo\cleaning services.xml";
            string ToLanguage = "german";

            string filePath = Path.GetFullPath(OriginalFullPathName);
            string OriginalxmlFile = File.ReadAllText(filePath);
            Task<string> task = new LanguageService.LanguageClass(blobConnectionString, containerName).ConvertXml2Html(OriginalxmlFile, ToLanguage);
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

        [TestMethod]
        public void TestGetXslt4Language()
        {
            string result = new LanguageClass(blobConnectionString, containerName).GetXslt4Language("French");
            Assert.IsTrue(result.Substring(0, 5) == "<?xml");
        }

        [TestMethod]
        public void TestTranslate()
        {
            string textToTranslate = "Hello world";
            string toLanguage = "German";
            string fromLanguage = "English";
            Task<string> translation = new LanguageClass(blobConnectionString, containerName).translate(textToTranslate, toLanguage, fromLanguage);

            translation.Wait();
            Assert.AreNotEqual(translation, "");
        }

        [TestMethod]
        public void TestGetXml4Language()
        {
            string result = new LanguageClass(blobConnectionString, containerName).GetTranslatedXml("Finnish");
            Assert.AreNotEqual(result, "");
        }

        [TestMethod]
        public void TestFillLanguages()
        {
            List<string> result = new LanguageClass(blobConnectionString, containerName).FillLanguages();
            Assert.AreNotEqual(result, "");
            
        }
        #region Web API unit tests
        [TestMethod]
        public void TestDownloadXmlFromWebAPI()
        {
            string uri = "https://localhost:44330/api/";
            string action =  "fileapi/gettranslatedxml/";
            string parameter = "?language=French";
            Task<string> res = new CallWebApi().GetStringFromWebAPI(uri, action, parameter);
            res.Wait();
            Assert.IsTrue(res.Result.Contains("xml"));
        }

        [TestMethod]
        public void TestUploadFileToWebAPI()
        {
            string uri = baseUri;
            string route = "/api/fileapi/postfile/";
            string requestBody = "{\"FileName\": \"Test2.txt\", \"FileContents\": \"This is another test\"}";
            Task<bool> res = new CallWebApi().CallPostWebAPI(uri, route, requestBody);
            res.Wait();
            Assert.IsTrue(res.Result);
        }

        [TestMethod]
        public void TestTranslateFileFromWebAPI()
        {
            string uri = baseUri;
            string route = "/api/translate/";
            string requestBody = "{\"ToLanguage\": \"French\", \"FromLanguage\": \"English\", \"TextToTranslate\": \"Hello\"}";
            Task<bool> res = new CallWebApi().CallPostWebAPI(uri, route, requestBody);
            res.Wait();
            Assert.IsTrue(res.Result);
        }

        [TestMethod]
        public void TestConvertXml2Html()
        {
            string uri = baseUri;
            string route = "/api/convertxml2html";

            string OriginalxmlFile = File.ReadAllText(@"..\..\..\MSTranslatordemo\cleaning services.xml");
            ToTranslate toTranslate = new ToTranslate();
            toTranslate.ToLanguage = "Greek";
            toTranslate.FromLanguage = "English";
            toTranslate.TextToTranslate = OriginalxmlFile;
            Task<string> res = new CallWebApi().PostWebAPIToTranslate<ToTranslate>(uri, route, toTranslate);
            res.Wait();
            File.WriteAllText("eInvoice.html", res.Result);
            System.Diagnostics.Process.Start("eInvoice.html");
            Assert.AreNotEqual(res.Result, "");
        }

        [TestMethod]
        public void TestDownloadxslt()
        {           
            string uri = "https://localhost:44330/api/";
            string route = "fileapi/xsltfile/";
            string parameter = "?language=French";
            Task<string> res = new CallWebApi().CallGetWebAPI(uri, route, parameter);
            res.Wait();
            Assert.AreNotEqual(res.Result, "");
        }

        #endregion
    }
}