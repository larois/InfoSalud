using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using Nokia.Phone.HereLaunchers;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Globalization;

namespace Sen.HTMLParser
{
    public partial class DetalleHospital : PhoneApplicationPage
    {
        public DetalleHospital()
        {
            InitializeComponent();

            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "91f8a3ae-5b38-425c-9985-9c0de535815e";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "iWu5NuU7uEh9GfKfpIOOCg";
        }

        string Parametros, voyLat,voyLon;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            NavigationContext.QueryString.TryGetValue("sParametro", out Parametros);

            string[] para = Parametros.Split('|');

            string region = para[0];
            string comuna = para[1];
            string departamento = para[2];
            string tipo = para[3];
            string nombre = para[4];
            string direccion = para[5];


            string[] data2 = App.voy.Split('|'); 
            voyLat = data2[0].ToString();
            voyLon = data2[1].ToString();

            txtNombre.Text = nombre;
            txtTipo.Text = tipo;
            txtDepartamento.Text = departamento;
            txtComuna.Text = comuna;
            txtDireccion.Text = direccion;
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