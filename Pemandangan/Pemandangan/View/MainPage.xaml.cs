using Pemandangan.Model;
using Pemandangan.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pemandangan
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private string lang;
        public MainPage()
        {
            this.InitializeComponent();
            lang = (string)LanguagePage.LOCAL_SETTINGS.Values["Language"];

            if (lang == "en")
                PageName.Text = "Language";
            else
                PageName.Text = "Taal";

            innerFrame.Navigate(typeof(LanguagePage), entireFrame);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
            if (SelectRoute.IsSelected)
            {
                if (lang != null && lang == "en")
                    PageName.Text = "Map";
                else
                    PageName.Text = "Kaart";

                innerFrame.Navigate(typeof(RoutePage), innerFrame);
            }
            else if (Map.IsSelected)
            {
                if (lang != null && lang == "en")
                    PageName.Text = "Map";
                else
                    PageName.Text = "Kaart";

                innerFrame.Navigate(typeof(MapPage), new Tuple<Frame, DataHandler>(innerFrame, null));
            }
            else if (Help.IsSelected)
            {
                if (lang != null && lang == "en")
                    PageName.Text = "Help";
                else
                    PageName.Text = "Help";

                innerFrame.Navigate(typeof(HelpPage));
            }
            else if (Language.IsSelected)
            {
                if (lang != null && lang == "en")
                    PageName.Text = "Language";
                else
                    PageName.Text = "Taal";

                innerFrame.Navigate(typeof(LanguagePage), entireFrame);
            }
            else if (RouteReset.IsSelected)
            {
                PageName.Text = "Route Reset";
                innerFrame.Navigate(typeof(ResetPage), entireFrame);
            }
        }
    }
}
