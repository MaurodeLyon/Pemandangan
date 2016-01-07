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
        public MainPage()
        {
            this.InitializeComponent();
            PageName.Text = "Language";
            myFrame.Navigate(typeof(LanguagePage),RootFrame);
            //startUpLanguage();
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
                PageName.Text = "Map";
                myFrame.Navigate(typeof(RoutePage),myFrame);
            }
            else if (Map.IsSelected)
            {
                PageName.Text = "Map";
                myFrame.Navigate(typeof(MapPage));
            }
            else if (Help.IsSelected)
            {
                PageName.Text = "Help";
                myFrame.Navigate(typeof(HelpPage));
            }
            else if (Language.IsSelected)
            {
                PageName.Text = "Language";
                myFrame.Navigate(typeof(LanguagePage),RootFrame);
            }
            else if (RouteReset.IsSelected)
            {
                PageName.Text = "Route Reset";
                myFrame.Navigate(typeof(ResetPage));
            }
        }
    }
}
