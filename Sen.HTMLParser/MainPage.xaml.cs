using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Sen.HTMLParser.Resources;
using System.Collections.ObjectModel;
using HtmlAgilityPack;
using System.IO.IsolatedStorage;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;
using WindowsPreview.Media.Ocr;
using Microsoft.Devices;
using Microsoft.Xna.Framework.Media;
using Facebook;
using Facebook.Client;

namespace Sen.HTMLParser
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += this.MainPage_Loaded; 
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Terminate();
        }


        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings sesion = IsolatedStorageSettings.ApplicationSettings;
            var session = SessionStorage.Load();


            if (!IsolatedStorageSettings.ApplicationSettings.Contains("faceName"))
            {
                if (null != session)
                {
                    try
                    {
                        var fb = new FacebookClient(session.AccessToken);

                        dynamic result = await fb.GetTaskAsync("me");
                        var user = new GraphUser(result);

                        sesion.Add("faceName", user.Name);
                        sesion.Add("faceBorn", user.Birthday);
                        
                        sesion.Save();

                        busyIndicator.IsRunning = false;
                    }
                    catch (FacebookOAuthException exception)
                    {
                        MessageBox.Show("Error fetching user data: " + exception.Message);
                    }
                }
            }
            else {
                busyIndicator.IsRunning = false;
            }
           
            
        }

        private void Take_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                WriteableBitmap bgraBitmap = new WriteableBitmap(fotoQla, null);//this.GetBgraBitmapFormat(this.imgbus);

                if (bgraBitmap.Equals(null))
                {
                    return;
                }
                this.ExtractText(bgraBitmap.ToByteArray(), bgraBitmap.PixelWidth, bgraBitmap.PixelHeight);
            }
            catch
            {

            }
        }

        private WriteableBitmap GetBgraBitmapFormat(Image img)
        {
            BitmapImage bitmap = img.Source as BitmapImage;

            WriteableBitmap bgraBitmap = new WriteableBitmap(bitmap);

            return bgraBitmap;
        }

        private async void ExtractText(byte[] bytePixels, int pixelsWidth, int pixelsHeight)
        {
            try
            {
                OcrEngine ocrEngine = new OcrEngine(OcrLanguage.English);

                var ocrResult = await ocrEngine.RecognizeAsync((uint)pixelsHeight, (uint)pixelsWidth, bytePixels);

                string extractedText = string.Empty;

                foreach (var line in ocrResult.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        extractedText += word.Text + " ";
                    }
                    extractedText += System.Environment.NewLine;
                } 
                string[] primer = extractedText.Split(' ');
                string[] second = primer[0].Split('.');
                string[] thrird = second[0].Split('\n');

                var medicamento = OnlyLetters(thrird[0]);

                MessageBoxResult result = MessageBox.Show(""+medicamento, "¿ Desea buscar ?", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/PivotPage1.xaml?sNombre=" + medicamento, UriKind.Relative));
                }
            }
            catch
            { 
                MessageBox.Show("Intenta nuevamente porfavor", "Error", MessageBoxButton.OK);
            }
        }

        public static string OnlyLetters(string input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var element in input)
            {
                if (char.IsLetter(element))
                {
                    builder.Append(element);
                }
            }
            return builder.ToString();
        }

        PhotoCamera cam;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) || (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                if (PhotoCamera.IsCameraTypeSupported(CameraType.Primary))
                {
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }
                else
                {
                    MessageBox.Show("Solo para camara trasera", "Error", MessageBoxButton.OK);
                }

                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);
                cam.AutoFocusCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_AutoFocusCompleted);
                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);
                cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);

                viewfinderBrush.SetSource(cam);
            }
            else
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    MessageBox.Show("No posee una camara disponible", "Error", MessageBoxButton.OK);
                });
                btnTake.IsEnabled = false;
            }
        }

        private void cam_AutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e){}

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                cam.Dispose();

                cam.Initialized -= cam_Initialized;
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
                cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
                cam.AutoFocusCompleted -= cam_AutoFocusCompleted;
            }
        }


        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                { });
            }
        }


        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {

        }

        public void cam_CaptureThumbnailAvailable(object sender, ContentReadyEventArgs e){}

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e){}



        private void buscar_Click(object sender, RoutedEventArgs e)
        {
            if (txtNombre.Text.Length > 2)
            {
                NavigationService.Navigate(new Uri("/PivotPage1.xaml?sNombre=" + txtNombre.Text, UriKind.Relative));
            }
            else 
            {
                MessageBox.Show("Ingresa algun medicamento valido", "Error", MessageBoxButton.OK);
            }
        }

        private void hospitales_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Hopitales.xaml", UriKind.Relative));
        }

        private void Drugstore_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Drugstore.xaml", UriKind.Relative));
        }

        private void cerrar_Click(object sender, EventArgs e)
        {
            IsolatedStorageSettings sesion = IsolatedStorageSettings.ApplicationSettings;
            sesion.Remove("faceName");
            sesion.Remove("faceBorn");
            sesion.Remove("faceRut");
            sesion.Save();

            SessionStorage.Remove();
            NavigationService.GoBack();
        }

     




      
       

       


    }
}