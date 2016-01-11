using Newtonsoft.Json;
using Pemandangan.Model;
using Pemandangan.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;
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
        private DataHandler dataHandler;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            dataHandler = (DataHandler)e.Parameter;
            ApplicationDataContainer Local_Settings = ApplicationData.Current.LocalSettings;
            try
            {
                bool wasWalking = Convert.ToBoolean(Local_Settings.Values["isWalking"]);
                if (wasWalking)
                {
                    System.Diagnostics.Debug.WriteLine("was lope");
                    string jsonString = "";
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Loading walked route");
                        using (var stream = ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("walkedRoute.json").Result)
                        {
                            byte[] result = new byte[stream.Length];
                            stream.ReadAsync(result, 0, result.Length);
                            jsonString = Encoding.ASCII.GetString(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("not yet ");
                    }
                    if (!string.IsNullOrEmpty(jsonString))
                        dataHandler.walkedRoute = JsonConvert.DeserializeObject<List<Model.Point>>(jsonString);
                    System.Diagnostics.Debug.WriteLine("size: " + dataHandler.walkedRoute.Count);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("niet lope");
                }
            }
            catch (NullReferenceException ex) { }  //First run
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

                innerFrame.Navigate(typeof(RoutePage), new Tuple<Frame, DataHandler>(innerFrame, dataHandler));
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
