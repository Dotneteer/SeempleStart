using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Younderwater.Webclient.Models.UserManagement
{
    /// <summary>
    /// This class defines a default SMS service that does not send any SMS
    /// </summary>
    public class DefaultSmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
        }
    }

}