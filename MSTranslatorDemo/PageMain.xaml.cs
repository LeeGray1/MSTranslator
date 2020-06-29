using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using Saxon.Api;
using Microsoft.Win32;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Xml.Xsl;
using System.Xml;
using System.Net;
using System.IO;
using System.Net.Http;
using LanguageService;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        const string blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=mstranslation;AccountKey=DhlfSrT66vg/I5CwpD0WrpeviWp5jrv/eyPaSTt7Pe8I0rv1PJnD3j8I7gGyc8oP0Jxs1+OpaL0U8Ku7kjFlFQ==;EndpointSuffix=core.windows.net";
        const string containerName = "xsltstorage";
        const string baseUri = "https://localhost:44330/";
        HttpClient httpClient = new HttpClient();
        public PageMain()
        {
            InitializeComponent();
            // Populate drop-downs with values from GetLanguagesForTranslate
            PopulateLanguageCombos();
            btnSave_XML.IsEnabled = false;
            btnSaveXSLT.IsEnabled = false;           
        }
       
        private void PopulateLanguageCombos()
        {           
            ToLanguageComboBox.ItemsSource = new LanguageClass(blobConnectionString, containerName).FillLanguages();
            // Set default languages           
            ToLanguageComboBox.SelectedItem = "English";
        }

        private async void btnCreateInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (btnCreateInvoice.IsEnabled == false)
            {
                MessageBox.Show("no xml file loaded");
                return;
            }

            btnSaveXSLT.IsEnabled = false;
            btnSave_XML.IsEnabled = false;

            string OriginalxmlFile = File.ReadAllText(openFileDialog.FileName);
            if (ToLanguageComboBox.Text == "English")
            {
                MessageBox.Show("Please select another language", "Translation not supported", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            ToTranslate toTranslate = new ToTranslate();
            toTranslate.TextToTranslate = OriginalxmlFile;
            toTranslate.ToLanguage = ToLanguageComboBox.Text;
            //string HTMLstring = await new LanguageClass(blobConnectionString, containerName).ConvertXml2Html(OriginalxmlFile, ToLanguageComboBox.Text);
            string uri = baseUri;
            string route = "/api/convertxml2html";
            string HTMLstring = await new WebAPIHandler(httpClient).PostWebAPIToTranslate<ToTranslate>(uri, route, toTranslate);
            File.WriteAllText("eInvoice.html", HTMLstring);
            System.Diagnostics.Process.Start("eInvoice.html");

            btnSaveXSLT.IsEnabled = true;
            btnSave_XML.IsEnabled = true;
        }

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "xml eInvoice file (*.xml)|*.xml|All files (*.*)|*.*",
        };

        private void UploadXml_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                XML_File_txtbx.Text = Path.GetFileName(openFileDialog.FileName);
                btnCreateInvoice.IsEnabled = true;
                btnSaveXSLT.IsEnabled = false;
                btnSave_XML.IsEnabled = false;

            }
            else
                XML_File_txtbx.Text = "no file selected";
        }

        private async void btnSave_XML_Click(object sender, RoutedEventArgs e)
        {          
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = ToLanguageComboBox.Text + "-" + Path.GetFileName(openFileDialog.FileName);
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML eInvoice (.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                string uri = baseUri;
                string route = "api/fileapi/gettranslatedxml/";
                string parameter = "?language=" + ToLanguageComboBox.Text;
                string xmlFile = await new WebAPIHandler(httpClient).GetStringFromWebAPI(uri, route, parameter);
                //xmlFile.Wait();

                //string xmlfile = new LanguageClass(blobConnectionString, containerName).GetTranslatedXml(ToLanguageComboBox.Text);
                File.WriteAllText(dlg.FileName, xmlFile);
            }
        }

        private async void btnSaveXSLT_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = ToLanguageComboBox.Text + "-stylesheet-ubl.xslt";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xslt Stylesheet (.xslt)|*.xslt";
            if (dlg.ShowDialog() == true)
            {
                string uri = baseUri;
                string route = "api/fileapi/xsltfile/";
                string parameter = "?language=French";
                string xsltFile = await new WebAPIHandler(httpClient).CallGetXsltFromWebAPI(uri, route, parameter);
                //res.Wait();
                //string xsltfile = new LanguageClass(blobConnectionString, containerName).GetXslt4Language(ToLanguageComboBox.Text);
                File.WriteAllText(dlg.FileName, xsltFile);
            }
        }
    }
}