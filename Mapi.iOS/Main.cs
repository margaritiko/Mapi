using UIKit;

namespace Mapi.iOS
{
    public class Application
    {
        static void Main(string[] args)
        {
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch
            {
                App.router.ChangePageTo(new Mapi.Views.MainMap());
            }
        }
    }
}
