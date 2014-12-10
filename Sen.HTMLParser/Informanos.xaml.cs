using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.Text;

namespace Sen.HTMLParser
{
    public partial class Informanos : PhoneApplicationPage
    {
        public Informanos()
        {
            InitializeComponent();

            this.listPicker2.ItemsSource = new Motivos[] { 
                new Motivos("Reclamo"), 
                new Motivos("Sugerencia"), 
                new Motivos("Felicitacion"), 
                new Motivos("Otro"), 
            };
        }

        string Parametros;
        IsolatedStorageSettings sesion = IsolatedStorageSettings.ApplicationSettings;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationContext.QueryString.TryGetValue("sParametro", out Parametros);

            string[] data = Parametros.Split('|');

            MessageBox.Show(Parametros);
            string nombre = data[0];
            string direccion = data[1];
            string ciudad = data[2];

            txtNombre.Text = nombre;
            txtDireccion.Text = direccion;

            try
            {
                txtMail.Text = sesion["faceMail"].ToString();
            }
            catch { }
        }

        private void Info_Click(object sender, EventArgs e)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("faceMail"))
            {
                sesion.Add("faceMail", txtMail.Text);
                sesion.Save();
            }
            else 
            {
                sesion["faceMail"] = txtMail.Text;
                sesion.Save();
            }

            if (txtMail.Text.Length < 5)
            {
                MessageBoxResult result = MessageBox.Show("Por favor ingrese un Email valido", "Error", MessageBoxButton.OKCancel);
            }
            else 
            {
                if (txtComentarios.Text.Length < 20)
                {
                    MessageBoxResult result = MessageBox.Show("Su comentario debe ener mas de 10 caracteres", "Error", MessageBoxButton.OKCancel);
                }
                else 
                {
                    string motivo = "";
                    int asd = listPicker2.SelectedIndex;

                    if (asd == 0)
                        motivo = "Reclamo";

                    if (asd == 1)
                        motivo = "Sugerencia";

                    if (asd == 2)
                        motivo = "Felicitacion";

                    if (asd == 3)
                        motivo = "Otro";

                    string url = "http://www.infosalud.site90.net";
                    Uri uri = new Uri(url, UriKind.Absolute);

                    StringBuilder postData = new StringBuilder();
                    postData.AppendFormat("{0}={1}", "sRut", HttpUtility.UrlEncode(sesion["faceRut"].ToString()));
                    postData.AppendFormat("{0}={1}", "&sNombre", HttpUtility.UrlEncode(sesion["faceName"].ToString()));
                    postData.AppendFormat("{0}={1}", "&sMail", HttpUtility.UrlEncode(sesion["faceMail"].ToString()));
                    postData.AppendFormat("{0}={1}", "&sBorn", HttpUtility.UrlEncode(sesion["faceBorn"].ToString()));
                    postData.AppendFormat("{0}={1}", "&sFarmacia", HttpUtility.UrlEncode(txtNombre.Text));
                    postData.AppendFormat("{0}={1}", "&sDireccion", HttpUtility.UrlEncode(txtDireccion.Text));
                    postData.AppendFormat("{0}={1}", "&sReclamo", HttpUtility.UrlEncode(motivo));
                    postData.AppendFormat("{0}={1}", "&sComentario", HttpUtility.UrlEncode(txtComentarios.Text));


                    var webclient = new WebClient();
                    webclient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    webclient.Headers[HttpRequestHeader.ContentLength] = postData.Length.ToString();

                    webclient.UploadStringCompleted += client_UploadStringCompleted;
                    webclient.UploadProgressChanged += client_UploadProgressChanged;

                    //////////////
                    MessageBoxResult result = MessageBox.Show("Su comentario fue enviado exitosamente", "Exito", MessageBoxButton.OKCancel);
                    //////////////
                    webclient.UploadStringAsync(uri, "POST", postData.ToString());
                }
            }
        }

        private void client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e){}

        private void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e){}


        private void listPicker2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPicker2.SelectedIndex == 0)
            {
                txtComentarios.Text = "Bioequivalentes no disponibles ";
            }
            if (listPicker2.SelectedIndex == 1)
            {
                txtComentarios.Text = "Podrian haber ";
            }
            if (listPicker2.SelectedIndex == 2)
            {
                txtComentarios.Text = "Muy buena la atencion ";
            }
            if (listPicker2.SelectedIndex == 3)
            {
                txtComentarios.Text = "Otro : ";
            }
        }
    }

    public class Motivos
    {
        public Motivos(string xMotivo)
        {
            this.XMotivo = xMotivo;
        }
        
        public string XMotivo 
        { get; set; }
    }
}