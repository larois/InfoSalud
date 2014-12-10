using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Facebook.Client;
using Facebook;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.IO.IsolatedStorage;

namespace Sen.HTMLParser
{
    public partial class Login : PhoneApplicationPage
    {
        public Login()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private const string AppId = "1582438468644088";
        private GraphUser currentUser;
        private bool progressIsVisible;
        private string progressText;

        public GraphUser CurrentUser
        {
            get
            {
                return this.currentUser;
            }

            set
            {
                if (value != this.currentUser)
                {
                    this.currentUser = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public bool ProgressIsVisible
        {
            get
            {
                return this.progressIsVisible;
            }

            set
            {
                if (value != this.progressIsVisible)
                {
                    this.progressIsVisible = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public string ProgressText
        {
            get
            {
                return this.progressText;
            }

            set
            {
                if (value != this.progressText)
                {
                    this.progressText = value;
                    this.OnPropertyChanged();
                }
            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var session = SessionStorage.Load();

            if (null != session)
            {
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
      

        private void loginqlo_Click(object sender, RoutedEventArgs e)
        {
            //validar rut
            bool validador = false;
            if (txtDigito.Text.Length < 1 || txtVerificador.Text.Length < 1) 
            {
                MessageBox.Show("Ingrese RUN completo", "Error", MessageBoxButton.OK);
            }
            else
            {
                try
                {
                    validador = verificaRut(Convert.ToInt32(txtDigito.Text), txtVerificador.Text);

                    if (validador == true)
                    {
                        SessionStorage.Remove();

                        var rut= txtDigito.Text + "-" + txtVerificador.Text;
                        IsolatedStorageSettings sesion = IsolatedStorageSettings.ApplicationSettings;
                        sesion.Add("faceRut", rut);
                        
                        FacebookSessionClient fb = new FacebookSessionClient(AppId);
                        fb.LoginWithApp("public_profile, user_birthday", "custom_state_string");
                    }
                    else
                    {
                        MessageBox.Show("Rut no valido", "Error", MessageBoxButton.OK);
                    }
                }
                catch
                {
                    MessageBox.Show("Ingrese caracteres validos", "Error", MessageBoxButton.OK);
                }
            }
        }

        private bool verificaRut(int rut, string dv) 
        { 
            int Digito; int Contador; int Multiplo; int Acumulador; string RutDigito;
            Contador = 2; 
            Acumulador = 0;

            while (rut != 0) 
            { 
                Multiplo = (rut % 10) * Contador; 
                Acumulador = Acumulador + Multiplo; 
                rut = rut/10; 
                Contador = Contador + 1;

                if (Contador == 8) 
                { 
                    Contador = 2; 
                } 
            }

            Digito = 11 - (Acumulador % 11); 
            RutDigito = Digito.ToString().Trim(); 
            
            if (Digito == 10) 
            { 
                RutDigito = "K"; 
            } 
            if (Digito == 11) 
            { 
                RutDigito = "0"; 
            }

            if(RutDigito.ToString() == dv.ToString()) 
            { 
                return true; 
            }
            else
            { 
                return false; 
            } 
        }

      

    }
}