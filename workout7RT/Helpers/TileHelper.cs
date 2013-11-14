using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace workout7RT.Helpers
{
    static class TileHelper
    {//johny was here
        static public void SetUpTiles(int streak)
        {
            // wide tile 
            var largeTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideBlockAndText01);
            largeTile.GetElementsByTagName("text")[0].InnerText = "Workout 7";
            largeTile.GetElementsByTagName("text")[1].InnerText = "Your current streak is...";
            largeTile.GetElementsByTagName("text")[4].InnerText = streak.ToString();
            
            // medium tile
            var mediumTile = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareBlock);
            mediumTile.GetElementsByTagName("text")[0].InnerText = streak.ToString();
            mediumTile.GetElementsByTagName("text")[1].InnerText = "days in a row";

            var node = largeTile.ImportNode(mediumTile.GetElementsByTagName("binding").Item(0), true);
            largeTile.GetElementsByTagName("visual").Item(0).AppendChild(node);

            var tileNotification = new TileNotification(largeTile); // { ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(10) };    

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
        }

        static public void ShowToastNotification(int streak)
        {
            // to do. 

            var xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText01);
            xml.GetElementsByTagName("text")[0].AppendChild(xml.CreateTextNode("Hello from toast!"));
            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(xml));
        }


    }
}
