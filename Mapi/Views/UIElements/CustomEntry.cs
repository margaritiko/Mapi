using Xamarin.Forms;

namespace Mapi
{
    public class CustomEntry: Entry
    {
        public CustomEntry()
        {
            HeightRequest = 55;
            this.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeNone);
        }
    }
}
