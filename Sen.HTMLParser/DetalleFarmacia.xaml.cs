using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Nokia.Phone.HereLaunchers;
using System.Device.Location;
using System.Globalization;

namespace Sen.HTMLParser
{
    public partial class DetalleFarmacia : PhoneApplicationPage
    {
        public DetalleFarmacia()
        {
            InitializeComponent();

            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "91f8a3ae-5b38-425c-9985-9c0de535815e";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "iWu5NuU7uEh9GfKfpIOOCg";
        }

        string Parametros, voyLat, voyLon;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationContext.QueryString.TryGetValue("sParametro", out Parametros);

            string[] data = Parametros.Split('|');

            string nombre = data[0];
            string direccion = data[1];
            string ciudad = data[2];
            string apertura = data[3];
            string cierre = data[4];
            string telefono = data[5];

            string[] data2 = App.voy.Split('|');
            voyLat = data2[0].ToString();
            voyLon = data2[1].ToString();

            txtNombre.Text = nombre;
            txtDireccion.Text = direccion;
            txtCuidad.Text = ciudad;
            txtApertura.Text = apertura;
            txtCierre.Text = cierre;
            txtTelefono.Content = "+"+telefono.Trim();

            Parametros = nombre + "|" + direccion + "|" + ciudad;
        }

        private void Info_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Informanos.xaml?sParametro="+Parametros, UriKind.Relative));
        }

        private void call_Click(object sender, RoutedEventArgs e)
        {
            PhoneCallTask call = new PhoneCallTask();
            call.DisplayName = txtNombre.Text;
            call.PhoneNumber = txtTelefono.Content+"";
            call.Show();
        }

        private void como_Click(object sender, EventArgs e)
        {
            DirectionsRouteDestinationTask routeTask = new DirectionsRouteDestinationTask();
            routeTask.Origin = new GeoCoordinate(Convert.ToDouble(App.miLat.Replace(',', '.'), CultureInfo.InvariantCulture), Convert.ToDouble(App.miLon.Replace(',', '.'), CultureInfo.InvariantCulture));
            routeTask.Destination = new GeoCoordinate(Convert.ToDouble(voyLat.Replace(',', '.'), CultureInfo.InvariantCulture), Convert.ToDouble(voyLon.Replace(',', '.'), CultureInfo.InvariantCulture));

            routeTask.Show();
        }
    }
}