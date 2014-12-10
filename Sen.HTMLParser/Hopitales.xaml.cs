using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Globalization;

namespace Sen.HTMLParser
{
    public partial class Hopitales : PhoneApplicationPage
    {
        public Hopitales()
        {
            InitializeComponent();
            miPosicion();
            cargar();
        }

        private GeoCoordinateWatcher Watcher { get; set; }
        MapLayer locationLayer = null;
        public void miPosicion()
        {
            this.Watcher = new GeoCoordinateWatcher();
            this.Watcher.MovementThreshold = 30;
            this.Watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            this.Watcher.Start();
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (locationLayer != null)
                Mapita.Layers.Remove(locationLayer);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("/Images/Location.png", UriKind.Relative));
            MapOverlay overlay = new MapOverlay

            {
                Content = new System.Windows.Shapes.Rectangle()
                {
                    Fill = ib,
                    Width = 60,
                    Height = 60
                }
            };
            locationLayer = new MapLayer();
            locationLayer.Add(overlay);
            overlay.GeoCoordinate = e.Position.Location;

            App.miLat = e.Position.Location.Latitude+"";
            App.miLon = e.Position.Location.Longitude+"";

            Mapita.SetView(e.Position.Location, 14);
            Mapita.Layers.Add(locationLayer);
        }

       
        public void cargar()
        {
            List<Hospinica> Far = new List<Hospinica>();
            XDocument objXML = XDocument.Load("Hopsitales.xml");

            var data = (from query in objXML.Descendants("record")
                        //where query.Element("Row").Attribute("A").Value.Contains("13")
                        select new Hospinica
                        {
                            NumRegion = (string)query.Element("Row").Attribute("A").Value,
                            Region = (string)query.Element("Row").Attribute("C").Value,
                            Comuna = (string)query.Element("Row").Attribute("D").Value,
                            Departamento = (string)query.Element("Row").Attribute("E").Value,
                            Tipo = (string)query.Element("Row").Attribute("F").Value,
                            Nombre = (string)query.Element("Row").Attribute("G").Value,
                            Direccion = (string)query.Element("Row").Attribute("H").Value,
                            Coordenada = (string)query.Element("Row").Attribute("J").Value,
                        }).ToArray();

            double latitud, longitud;
            foreach (Hospinica aux in data)
            {
                string[] coor = aux.Coordenada.Split(',');

                latitud = Convert.ToDouble(coor[0], CultureInfo.InvariantCulture);
                longitud = Convert.ToDouble(coor[1], CultureInfo.InvariantCulture);

                    Image img = new Image { Width = 46, Height = 46 };
                    img.Source = new BitmapImage(new Uri("/Images/Ambulance.png", UriKind.Relative));
                    
                    img.Tag = aux.Region + "|" + aux.Comuna + "|" + aux.Departamento + "|" + aux.Tipo + "|" + aux.Nombre + "|" + aux.Direccion;
                    img.Name = latitud + "|" + longitud;
                    img.Tap += img_Tap;



                    MapOverlay overlay = new MapOverlay
                    {
                        GeoCoordinate = new GeoCoordinate(latitud, longitud),
                        Content = img
                    };

                    MapLayer layer = new MapLayer();
                    layer.Add(overlay);
                    Mapita.Layers.Add(layer);
           
            }

            busyIndicator.IsRunning = false;
        }

        void img_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image asd = (Image)sender;
            App.voy = asd.Name;
            NavigationService.Navigate(new Uri("/DetalleHospital.xaml?sParametro=" + asd.Tag, UriKind.Relative));
        }

        private void Mapita_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "91f8a3ae-5b38-425c-9985-9c0de535815e";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "iWu5NuU7uEh9GfKfpIOOCg";
        }
    }
}