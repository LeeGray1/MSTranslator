﻿using System;
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
using System.Windows.Shapes;

namespace MSTranslatorDemo
{
    /// <summary>
    /// Interaction logic for InvoiceDisplay.xaml
    /// </summary>
    public partial class InvoiceDisplay : Window
    {
        public InvoiceDisplay()
        {
            InitializeComponent();
        }

        private void wbInvoice_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());

            e.Cancel = true;
        }
    }
}
