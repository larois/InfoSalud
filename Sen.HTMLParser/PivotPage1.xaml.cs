using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Windows.Media;
using Microsoft.Phone.Tasks;
using System.Net.Http;

namespace Sen.HTMLParser
{
    public partial class PivotPage1 : PhoneApplicationPage
    {
        string Nombre;
        public PivotPage1()
        {
            InitializeComponent();
            Bioequivalencia();
        }

        public void MedicamentosBio()
        {
            try
            {
                List<BioequivalenteClass> Bio = new List<BioequivalenteClass>();
                XDocument objXML = XDocument.Load("bioequivalentes.xml");

                var data = (from query in objXML.Descendants("Record")
                            where query.Element("Row").Attribute("A").Value.Contains(Nombre)
                            select new NOSE
                            {
                                variable = (string)query.Element("Row").Attribute("A").Value
                            }).ToArray();

                foreach (NOSE aux in data)
                {
                    string[] nom = aux.variable.Split(',');

                    txtNombre.Text = nom[1].ToString();
                    txtUso.Text = nom[7].ToString();

                    Bio.Add(new BioequivalenteClass { Laboratorio = nom[4].ToString(), Composicion = nom[2].ToString() });
                }

                var a = Bio.Count;
                if (a < 1)
                {
                    txtNombre.Text = Nombre.ToUpper();
                    txtUso.Text = "No encontrado";

                    TextBlock nope = new TextBlock();
                    nope.Text = " ¡ No existen Bioequivalentes asosiados al medicamento buscado ! ";
                    nope.TextWrapping = TextWrapping.Wrap;
                    nope.Margin = new Thickness(10, 0, 10, 0);
                    nope.FontSize = 25;
                    nope.Foreground = new SolidColorBrush(Color.FromArgb(255, 89, 176, 244));
                    stkNope.Children.Add(nope);
                }

                listDetail.ItemsSource = Bio;

                busyIndicator.IsRunning = false;
            }
            catch {
                MessageBoxResult result = MessageBox.Show(".......................",
                    "Error", MessageBoxButton.OK);

                busyIndicator.IsRunning = false;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);
                NavigationContext.QueryString.TryGetValue("sNombre", out Nombre);
                Nombre = Nombre.ToUpper();
                string htmlPage;

                using (var client = new HttpClient())
                {
                    htmlPage = await client.GetStringAsync("http://cl.kairosweb.com/resultado-busqueda.php?prodname=" + Nombre);
                }

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(htmlPage);

                var dato = htmlDocument.DocumentNode.SelectSingleNode("//table[@width='100%'][2]");

                bool i = false;
                int cont = 0, datos = 0;
                HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectNodes("//tr");

                foreach (HtmlNode link in collection)
                {
                    if (i)
                    {
                        StackPanel stk = new StackPanel();
                        StackPanel nose = new StackPanel();

                        TextBlock text1 = new TextBlock();
                        TextBlock text2 = new TextBlock();

                        var r = link.Descendants("td").GetEnumerator();
                        r.MoveNext();
                        var valor = r.Current.InnerText.Trim();
                        if (cont > 28)
                        {
                            if (valor.Length > 2)
                            {
                                if (valor.Contains("&"))
                                {
                                    text1.Text = "";
                                    stk.Children.Add(text1);

                                }
                                else
                                    if (!valor.Contains("PRESENTACION"))
                                    {
                                        text1.Text = valor;
                                        if (valor.Contains(Nombre.ToUpper()))
                                        {
                                            text1.Margin = new Thickness(20, 0, 0, 0);
                                            text1.Foreground = new SolidColorBrush(Color.FromArgb(255, 100, 100, 100));
                                        }
                                        else
                                        {
                                            text1.Margin = new Thickness(50, 0, 0, 0);
                                            text1.Foreground = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));

                                        }

                                        stk.Children.Add(text1);
                                    }
                            }
                        }
                        else
                            cont++;



                        r.MoveNext();
                        var aaa = r.Current.InnerText.Trim();
                        if (cont > 28)
                        {
                            if (aaa.Length < 2 || aaa.Equals(valor)) { }
                            else
                            {
                                text2.Text = "   $ " + Convert.ToInt32(aaa.Replace(".00", "").Replace(",", ""));
                                text2.Margin = new Thickness(50, 0, 0, 0);
                                text2.Foreground = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
                                stk.Children.Add(text2);
                            }
                        }
                        else
                            cont++;

                        if (cont > 28)
                        {
                            datos++;
                            stkPanel.Children.Add(stk);
                        }
                    }
                    else
                    {
                        i = true;

                    }
                }

                if (datos < 1)
                {
                    MessageBoxResult result = MessageBox.Show("No se encontro el medicamento buscado... ",
                    "Error", MessageBoxButton.OK);

                    if (result == MessageBoxResult.OK)
                        NavigationService.GoBack();
                }

            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Error de conexion... ",
                    "Error", MessageBoxButton.OK);

                NavigationService.GoBack();
            }

            MedicamentosBio();
        }

        private void info_Click(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("\nPrecios :\nLos precios asociados a cada medicamento no quiere decir que sean "+
                "exactamente esos, por lo tanto pueden sufrir un margen, ya que las farmacias son las responsables de los precios.\n\n"+
                "Bioequivalentes :\nSu proposito es informar al publico que existen medicamentos con las mismas propiedades, salvo por su precio "+
                "que es mucho mas conveniente.\n\n"+
                "Informacion :\nLa informacion entregada sobre los bioequivalentes es recopilada del sitio oficial del Gobierno de Chile.",
                   "Info", MessageBoxButton.OK);            
        }

        private string[] capture = new string[3];
        private void Bioequivalencia()
        {
            LoopingListDataSource ds = new LoopingListDataSource(3);
            ds.ItemNeeded += this.OnDs_ItemNeeded;
            ds.ItemUpdated += this.OnDs_ItemUpdated;
            this.loopingList.DataSource = ds;
            this.loopingList.SelectedIndexChanged += this.OnSelectedIndexChanged;
            this.loopingList.SelectedIndex = 1;
        }

        private void OnSelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateCaption();
        }

        private void UpdateCaption()
        {
            int selectedIndex = this.loopingList.SelectedIndex;
        }

        private void OnDs_ItemUpdated(object sender, LoopingListDataItemEventArgs e)
        {
            if (e.Index == 0)
            {
                (e.Item as PictureLoopingItem).Url = "4Bpg8JFXNpk";
                (e.Item as PictureLoopingItem).Picture = new Uri("http://img.youtube.com/vi/4Bpg8JFXNpk/hqdefault.jpg", UriKind.Absolute);
            }

            if (e.Index == 1)
            {
                (e.Item as PictureLoopingItem).Url = "BLiqkXuVMhU";
                (e.Item as PictureLoopingItem).Picture = new Uri("http://img.youtube.com/vi/BLiqkXuVMhU/hqdefault.jpg", UriKind.Absolute);
            }

            if (e.Index == 2)
            {
                (e.Item as PictureLoopingItem).Url = "NCC4DBy0zeM";
                (e.Item as PictureLoopingItem).Picture = new Uri("http://img.youtube.com/vi/NCC4DBy0zeM/hqdefault.jpg", UriKind.Absolute);
            }

        }

        private void OnDs_ItemNeeded(object sender, LoopingListDataItemEventArgs e)
        {
            e.Item = new PictureLoopingItem() { Picture = new Uri("http://img.youtube.com/vi/NCC4DBy0zeM/hqdefault.jpg", UriKind.Absolute) };
        }

        private void Youtube_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image img = (Image)sender;
            var playbackUrl = "vnd.youtube:" + img.Tag;
            var task = new WebBrowserTask { URL = playbackUrl };
            task.Show();
        }
    }

    public class PictureLoopingItem : LoopingListDataItem
    {
        private Uri pictureSource;
        private string url;

        public string Url
        {
            get
            {
                return this.url;
            }
            set
            {
                if (value != this.url)
                {
                    this.url = value;
                    this.OnPropertyChanged("Url");
                }
            }
        }

        public Uri Picture
        {
            get
            {
                return this.pictureSource;
            }
            set
            {
                if (value != this.pictureSource)
                {
                    this.pictureSource = value;
                    this.OnPropertyChanged("Picture");
                }
            }
        }
    }
}