using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;
using LanguageTranslation;
using Azure.Storage.Blobs.Models;
using System.IO;
using Azure.Storage.Blobs;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestClassLibraryTest()
        {
            Class1 class1 = new Class1();
            string result = class1.test();
            Assert.IsTrue(result == "This works");
        }

        [TestMethod]
        public void TestAnotherClassLibraryTest()
        {
            Class1 class1 = new Class1();
            string result = class1.test();
            Assert.AreEqual(result, "This works");
        }

        [TestMethod]
        public void TestUpdateTranslation()
        {
            string result = new LanguageClass().UpdateTranslation("Address", "German","Adresse");
            Assert.AreNotEqual(result, "");
        }

        [TestMethod]
        public  void TestDownloadAzure()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            BlobContainerClient container = new BlobContainerClient(connectionString, "xsltstorage");
            var blockBlob = container.GetBlobClient("stylesheet-ubl v2.xslt");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Position = 0;
            blockBlob.DownloadTo(ms);
            string file = System.Text.Encoding.ASCII.GetString(ms.ToArray());           

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestUploadAzure()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=translation;AccountKey=89/llb7VuT1vV2XHTQbusAOeau/rFvzilR+REqnMLtMsnqRw7VLc9eSpt3fXRBRxRAyRnLdQ31H7VcsZgmu2zg==;EndpointSuffix=core.windows.net";
            BlobContainerClient container = new BlobContainerClient(connectionString, "xsltstorage");
            var blockBlob = container.GetBlobClient("stylesheet-ubl v2.xslt");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            string file = System.Text.Encoding.ASCII.GetString(ms.ToArray());
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(file);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);
            blockBlob.Upload(ms);
            Assert.IsTrue(true);
        }
    }
}
