﻿#pragma checksum "C:\Users\Jarek\Desktop\InfoSalud\Sen.HTMLParser\Drugstore.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3C46E2EB0E548B77F76C0ED07B2F5C77"
//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.34014
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Sen.HTMLParser {
    
    
    public partial class Drugstore : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.StackPanel LayoutRoot;
        
        internal Microsoft.Phone.Maps.Controls.Map Mapita;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Sen.HTMLParser;component/Drugstore.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.StackPanel)(this.FindName("LayoutRoot")));
            this.Mapita = ((Microsoft.Phone.Maps.Controls.Map)(this.FindName("Mapita")));
        }
    }
}

