﻿#pragma checksum "..\..\frmTranslateTest.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "81CDEF8261B67607163B31DFD8080EB2BF082B1DA42599D3C871EEB3FDE46687"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MSTranslatorDemo;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace MSTranslatorDemo {
    
    
    /// <summary>
    /// frmTranslateTest
    /// </summary>
    public partial class frmTranslateTest : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\frmTranslateTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TextToTranslate;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\frmTranslateTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button TranslateButton;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\frmTranslateTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label TranslatedTextLabel;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\frmTranslateTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox FromLanguageComboBox;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\frmTranslateTest.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox ToLanguageComboBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/MSTranslatorDemo;component/frmtranslatetest.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\frmTranslateTest.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.TextToTranslate = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.TranslateButton = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\frmTranslateTest.xaml"
            this.TranslateButton.Click += new System.Windows.RoutedEventHandler(this.TranslateButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.TranslatedTextLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.FromLanguageComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.ToLanguageComboBox = ((System.Windows.Controls.ComboBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

