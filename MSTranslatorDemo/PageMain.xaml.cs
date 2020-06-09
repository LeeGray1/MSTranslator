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
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Xml.Xsl;
using System.Xml;
using System.Net;
using System.IO;
using System.Net.Http;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for PageMain.xaml
    /// </summary>
    public partial class PageMain : Page
    {
        
        public PageMain()
        {
            InitializeComponent();
            // Populate drop-downs with values from GetLanguagesForTranslate
            PopulateLanguageMenus();
            btnSave_XML.IsEnabled = false;
            btnSaveXSLT.IsEnabled = false;
          
            
        }
       
        private void PopulateLanguageMenus()
        {
            // Add option to automatically detect the source language
            FromLanguageComboBox.Items.Add("Detect");
            List<string> list = new LanguageClass().FillLanguages();            

            ToLanguageComboBox.ItemsSource = list;

            // Set default languages
            
            ToLanguageComboBox.SelectedItem = "English";
        }       

        private async void btnLoadXML_Click(object sender, RoutedEventArgs e)
        {
            if (btnLoadXML.IsEnabled == false)
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
            string HTMLstring = await new LanguageClass().ConvertXml2Html(OriginalxmlFile, ToLanguageComboBox.Text, Path.GetFileName(openFileDialog.FileName));

            File.WriteAllText("eInvoice.html", HTMLstring);
            System.Diagnostics.Process.Start("eInvoice.html");

            btnSaveXSLT.IsEnabled = true;
            btnSave_XML.IsEnabled = true;


        }

        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "xml eInvoice file (*.xml)|*.xml|All files (*.*)|*.*",

        };

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == true)
            {
                XML_File_txtbx.Text = Path.GetFileName(openFileDialog.FileName);
                btnLoadXML.IsEnabled = true;
                btnSaveXSLT.IsEnabled = false;
                btnSave_XML.IsEnabled = false;

            }
            else
                XML_File_txtbx.Text = "no file selected";
        }

        private void btnSave_XML_Click(object sender, RoutedEventArgs e)
        {
            

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = ToLanguageComboBox.Text + "-" + Path.GetFileName(openFileDialog.FileName);
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML eInvoice (.xml)|*.xml";
            if (dlg.ShowDialog() == true)
            {
                string xmlfile = new LanguageClass().GetTranslatedXml(ToLanguageComboBox.Text);
                File.WriteAllText(dlg.FileName, xmlfile);
            }

        }

        private void btnSaveXSLT_Click(object sender, RoutedEventArgs e)
        {
            

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = ToLanguageComboBox.Text + "-stylesheet-ubl.xslt";
            dlg.DefaultExt = ".xml";
            dlg.Filter = "xslt Stylesheet (.xslt)|*.xslt";
            if (dlg.ShowDialog() == true)
            {
                string xsltfile = new LanguageClass().GetXslt4Language(ToLanguageComboBox.Text);
                File.WriteAllText(dlg.FileName, xsltfile);
            }
        }       
    }
}