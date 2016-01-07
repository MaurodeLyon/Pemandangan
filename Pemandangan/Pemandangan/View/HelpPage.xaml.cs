using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class HelpPage : Page
    {
        public HelpPage()
        {
            this.InitializeComponent();
            loadText();
        }

        void loadText()
        {
            string textNL = "Nederlands \r" +
               "Een route starten \r1. Ga via de NAV PANE naar de page Select Route \r2. Kies nu een van de voorgedefinieerde routes \r3. De route is nu gestart en u kunt beginnen met lopen \rOPMERKING: Als u niet beschikt over een GPS verbinding kunt u de route wel starten maar uw locatie zal niet worden weergegeven.\r\r" +
               "Taal veranderen \r1. Ga via de NAV PANE naar de page Language \r2. Kies nu de taal die u wilt gebruiken\r\r" +
               "Een route resetten \r1.Ga via de NAV PANE naar de page Route reset \r2.Druk op de knop Route reset\r\r" +
               "Uitleg icoontjes \rBlauw: dit is uw huidige positie \rRood: dit zijn punten waar u nog naar toe moet \rGrijs: dit zijn punten waar u al geweest bent\r\r";

            string textEN = "English \r" +
               "Start a route \r1. Go to the NAV PANE and select the page Select Route \r2. Choose one of the predefined routes \r3. The route has now started and you can start walking \rNOTE: If you do not have a GPS connection you can still walk the route but your location will not be shown.\r\r" +
               "Change language \r1. Go to the NAV PANE and select the page Language \r2. Now choose the language you would like to use\r\r" +
               "Reset a route \r1.Go to the NAV PANE and select the page Route reset \r2.Now click yes if you are sure you want to reset the route\r\r" +
               "Explanation icons \rBlue: this is your current location \rRed: these are points you still have to visit \rGrey: These are points you already have visited ";

            helpText.Text = textNL + "\r" + textEN;
        }
    }
}
