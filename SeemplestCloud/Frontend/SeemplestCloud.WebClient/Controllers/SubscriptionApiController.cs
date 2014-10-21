using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SeemplestBlocks.Core.ServiceInfrastructure;
using SeemplestCloud.Dto.Subscription;
using SeemplestCloud.Services.SubscriptionService;

namespace SeemplestCloud.WebClient.Controllers
{
    /// <summary>
    /// This API manages operations related to subscriptions
    /// </summary>
    [RoutePrefix("api/subscription")]
    [Authorize]
    public class SubscriptionApiController: ApiController
    {
        [HttpGet]
        [Route("invitations")]
        public async Task<List<UserInvitationCoreDto>> GetInvitedUsers()
        {
            var srvObj = HttpServiceFactory.CreateService<ISubscriptionService>();
            var result = await srvObj.GetInvitedUsers();
            return result;
        }

        [HttpPost]
        [Route("inviteUser")]
        public async Task InviteUserAsync(InviteUserDto userInfo)
        {
            var srvObj = HttpServiceFactory.CreateService<ISubscriptionService>();
            await srvObj.InviteUserAsync(userInfo);
        }
    }
}