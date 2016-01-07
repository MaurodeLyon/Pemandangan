﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Pemandangan.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LanguagePage : Page
    {
        ObservableCollection<String> list;
        private Frame frame;
        public ApplicationDataContainer LOCAL_SETTINGS = ApplicationData.Current.LocalSettings;

        public LanguagePage()
        {
            list = new ObservableCollection<string>();
            String lang = (String)LOCAL_SETTINGS.Values["Language"];
            if (lang != null && lang == "nl")
            {
                list.Add("Nederlands");
                list.Add("Engels");
            }
            else
            {
                list.Add("Dutch");
                list.Add("English");
            }
            this.InitializeComponent();
            
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox)sender;
            String lang = (String) box.SelectedItem;

            if ((String)LOCAL_SETTINGS.Values["Language"] == "nl")
            {
                switch (lang)
                {
                    case "Nederlands":
                        loadLanguage("nl");
                        LOCAL_SETTINGS.Values["Language"] = "nl";
                        break;
                    case "Engels":
                        loadLanguage("en");
                        LOCAL_SETTINGS.Values["Language"] = "en";
                        break;
                }
            }
            else
            {
                switch (lang)
                {
                    case "Dutch":
                        loadLanguage("nl");
                        LOCAL_SETTINGS.Values["Language"] = "nl";
                        break;
                    case "English":
                        loadLanguage("en");
                        LOCAL_SETTINGS.Values["Language"] = "en";
                        break;
                }
            }
        }

        private async void loadLanguage(String code)
        {
            ApplicationLanguages.PrimaryLanguageOverride = code;
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            frame.Navigate(typeof(MainPage));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            frame = (Frame)e.Parameter;
        }
    }
}
