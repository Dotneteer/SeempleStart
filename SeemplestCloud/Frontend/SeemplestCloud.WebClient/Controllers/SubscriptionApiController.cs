using System;
using System.Threading.Tasks;
using System.Web.Http;
using Seemplest.Core.ServiceObjects.Validation;
using SeemplestBlocks.Core.ServiceInfrastructure;
using SeemplestCloud.Dto.Subscription;
using SeemplestCloud.Services.SubscriptionService;

namespace SeemplestCloud.WebClient.Controllers
{
    /// <summary>
    /// This API manages operations related to subscriptions
    /// </summary>
    [RoutePrefix("api/subscription")]
    public class SubscriptionApiController: ApiController
    {
        [HttpPost]
        [Route("inviteUser")]
        [Authorize]
        public async Task InviteUserAsync(InviteUserDto userInfo)
        {
            var srvObj = HttpServiceFactory.CreateService<ISubscriptionService>();
            await srvObj.InviteUserAsync(userInfo);
        }
    }
}