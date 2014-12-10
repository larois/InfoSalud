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
using Microsoft.Phone.Maps;

namespace Sen.HTMLParser
{
    public partial class Drugstore : PhoneApplicationPage
    {
        public Drugstore()
        {
            InitializeComponent();
            miPosicion();
            //cargar();
            cargar2();
        }

        private void cargar2()
        {
            WebClient wc = new WebClient();
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            wc.DownloadStringAsync(new Uri("http://infosaludchile.site90.net/Mods/Jarek/ISFarmacia.xml"));
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                List<Farmacias> Far = new List<Farmacias>();
                XDocument objXML = XDocument.Parse(e.Result, LoadOptions.None);

                var data = (from query in objXML.Descendants("farmacia")
                            select new Farmacias
                            {
                                //Id = (string)query.Element("Id"),
                                Nombre = (string)query.Element("NombreFarmacia"),
                                Direccion = (string)query.Element("Direccion"),
                                Ciudad = (string)query.Element("Ciudad"),
                                Sector = (string)query.Element("Sector"),
                                Apertura = (string)query.Element("HoraApertura"),
                                Cierre = (string)query.Element("HoraCierre"),
                                Latitud = (string)query.Element("Latitud"),
                                Longitud = (string)query.Element("Longitud"),
                                Telefono = (string)query.Element("Telefono"),
                            }).ToArray();

                double latitud, longitud;
                int i = 0;
                string ctm1="", qlo1="";
                try{
                    var hora = Convert.ToInt32( DateTime.Now.Hour +""+ DateTime.Now.Minute);
                foreach (Farmacias aux in data)
                {
                    try
                    {
                        if (i != 0)
                        {
                            //string []minimo = aux.Apertura.Split(':');
                            //int minimin = Convert.ToInt32(minimo[0] + minimo[1]);

                            //string[] maximo = aux.Cierre.Split(':');
                            //int maxumun = Convert.ToInt32(maximo[0] + maximo[1]);

                            //if (hora > minimin && hora < maxumun)
                            //{

                                long b = aux.Latitud.LongCount(letra => letra.ToString() == ".");
                                long c = aux.Longitud.LongCount(letra => letra.ToString() == ".");

                                if (b > 1)
                                {
                                    string[] ctm = aux.Latitud.Split('.');
                                    ctm1 = ctm[0] + "." + ctm[1] + ctm[2];
                                }
                                else { ctm1 = aux.Latitud; }

                                if (c > 1)
                                {
                                    string[] qlo = aux.Longitud.Split('.');
                                    qlo1 = qlo[0] + "." + qlo[1] + qlo[2];
                                }
                                else { qlo1 = aux.Longitud; }

                                latitud = Convert.ToDouble(ctm1, CultureInfo.InvariantCulture);
                                longitud = Convert.ToDouble(qlo1, CultureInfo.InvariantCulture);

                                Image img = new Image { Width = 46, Height = 46 };
                                img.Source = new BitmapImage(new Uri("/Images/Pharmacy.png", UriKind.Relative));

                                img.Tag = aux.Nombre + "|" + aux.Direccion + "|" + aux.Ciudad + "|" + aux.Apertura + "|" + aux.Cierre + "|" + aux.Telefono;
                                img.Tap += img_Tap;
                                img.Name = latitud + "|" + longitud;


                                MapOverlay overlay = new MapOverlay
                                {
                                    GeoCoordinate = new GeoCoordinate(latitud, longitud),
                                    Content = img
                                };

                                MapLayer layer = new MapLayer();
                                layer.Add(overlay);
                                Mapita.Layers.Add(layer);
                            }
                        //}
                        else { i++; };
                    }
                    catch { }
                }
                }
                catch { }
                busyIndicator.IsRunning = false;
            }
            
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
            
            App.miLat = e.Position.Location.Latitude + "";
            App.miLon = e.Position.Location.Longitude + "";
            
            Mapita.SetView(e.Position.Location, 14);
            Mapita.Layers.Add(locationLayer);
        }

        public void cargar()
        {
            List<Farmacias> Far = new List<Farmacias>();
            XDocument objXML = XDocument.Load("Farmacias.xml");

            var data = (from query in objXML.Descendants("farmacia")
                        select new Farmacias
                        {
                            Id = (string)query.Element("Id"),
                            Nombre = (string)query.Element("NomFarm"),
                            Direccion = (string)query.Element("dir"),
                            Ciudad = (string)query.Element("ciudad"),
                            Sector = (string)query.Element("sector"),
                            Apertura = (string)query.Element("horaaper"),
                            Cierre = (string)query.Element("horacier"),
                            Latitud = (string)query.Element("lat"),
                            Longitud = (string)query.Element("lon"),
                            Telefono = (string)query.Element("Fono"),
                        }).ToArray();

            double latitud, longitud;
            foreach (Farmacias aux in data)
            {

                latitud = Convert.ToDouble(aux.Latitud, CultureInfo.InvariantCulture);
                longitud = Convert.ToDouble(aux.Longitud, CultureInfo.InvariantCulture);
                
                Image img = new Image { Width = 46, Height = 46 };
                img.Source = new BitmapImage(new Uri("/Images/Pharmacy.png", UriKind.Relative));
                
                img.Tag = aux.Nombre + "|" + aux.Direccion + "|" + aux.Ciudad + "|" + aux.Apertura + "|" + aux.Cierre + "|" + aux.Telefono;
                img.Tap += img_Tap;
                img.Name = latitud + "|" + longitud;

                
                MapOverlay overlay = new MapOverlay
                {
                    GeoCoordinate = new GeoCoordinate(latitud, longitud ),
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
            NavigationService.Navigate(new Uri("/DetalleFarmacia.xaml?sParametro=" + asd.Tag, UriKind.Relative));
        }

        private void Map_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "91f8a3ae-5b38-425c-9985-9c0de535815e";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "iWu5NuU7uEh9GfKfpIOOCg";
        }

    }
}