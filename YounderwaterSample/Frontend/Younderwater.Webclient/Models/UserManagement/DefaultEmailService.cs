using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Younderwater.Webclient.Models.UserManagement
{
    /// <summary>
    /// This class defines a default email service that does not send emails at all
    /// </summary>
    public class DefaultEmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            return Task.FromResult(0);
        }
    }

}