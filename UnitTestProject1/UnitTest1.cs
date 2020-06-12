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
        public  void TestAzure()
        {
            string downloadFilePath = "c:\test.txt";
            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=mstranslation;AccountKey=DhlfSrT66vg/I5CwpD0WrpeviWp5jrv/eyPaSTt7Pe8I0rv1PJnD3j8I7gGyc8oP0Jxs1+OpaL0U8Ku7kjFlFQ==;EndpointSuffix=core.windows.net");
            BlobContainerClient containerClient = blobServiceClient.CreateBlobContainer("xsltstorage");
            BlobClient blobClient = containerClient.GetBlobClient("mstranslation");

            BlobDownloadInfo download =  blobClient.Download();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                download.Content.CopyTo(downloadFileStream);
                downloadFileStream.Close();
            }

            Assert.IsTrue(true);
        }
    }
}
