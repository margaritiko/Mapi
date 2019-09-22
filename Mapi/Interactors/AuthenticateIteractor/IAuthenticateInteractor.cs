using System;
using Microsoft.WindowsAzure.MobileServices;

namespace Mapi
{
    public interface IAuthenticateInteractor
    {
        AuthenticateInteractor DefaultManager { get; }
    }
}
