using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Mapi
{
    public interface IAuthenticate
    {
        Task<object[]> AuthenticateAsync();
    }
}
