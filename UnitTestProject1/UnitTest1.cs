using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using LanguageService;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestUpdateTranslation()
        {
            string result = new LanguageClass().UpdateTranslation("Address", "German","Adresse");
            Assert.AreNotEqual(result, "");
        }

        [TestMethod]
        public  void TestDownloadAzure()
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
        public void TestUploadAzure()
        {
            string fileName = "Hindi-stylesheet-ubl.xslt";
            string filePath = @"C:\Users\edmun\source\repos\LeeGray1\MSTranslator\MSTranslatorDemo";
            string file = File.ReadAllText(Path.Combine(filePath, fileName));
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            string containerName = "xsltstorage";
            
            new LanguageClass().UploadFileToBlob(file, fileName, connectionString, containerName);
            Assert.IsTrue(true);
        }
       
    }
}
