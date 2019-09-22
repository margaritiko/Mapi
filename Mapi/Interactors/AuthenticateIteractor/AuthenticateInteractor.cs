using System;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace Mapi
{
    public class AuthenticateInteractor: IAuthenticateInteractor
    {
        static AuthenticateInteractor defaultInstance = new AuthenticateInteractor();

        public AuthenticateInteractor()
        {
            this.CurrentClient = new MobileServiceClient(Constants.ApplicationURL);
        }

        public AuthenticateInteractor DefaultManager
        {
            get
            {
                return defaultInstance;
            }
        }

        public MobileServiceClient CurrentClient { get; set; }
    }
}
