namespace Sen.HTMLParser
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Navigation;
    using Facebook.Client;
    using System.Threading.Tasks;

    public class CallbackUriMapper : UriMapperBase
    {
        private bool facebookLoginHandled;

        public override Uri MapUri(Uri uri)
        {
            if (AppAuthenticationHelper.IsFacebookLoginResponse(uri))
            {
                FacebookSession session = new FacebookSession();

                try
                {
                    session.ParseQueryString(HttpUtility.UrlDecode(uri.ToString()));

                    if (session.State != "custom_state_string")
                    {
                        MessageBox.Show("Unexpected state: " + session.State);
                    }
                    else
                    {
                        SessionStorage.Save(session);
                        //return new Uri("/MainPage.xaml", UriKind.Relative);
                    }
                }
                catch 
                {
                    if (!this.facebookLoginHandled)
                    {
                        // Handle error case
                        MessageBox.Show("No se pudo crear la sesion");

                        this.facebookLoginHandled = true;
                    }
                }

               return new Uri("/MainPage.xaml", UriKind.Relative);
            }

            // by default, navigate to the requested uri
            return uri;
        }

 
    }
}
