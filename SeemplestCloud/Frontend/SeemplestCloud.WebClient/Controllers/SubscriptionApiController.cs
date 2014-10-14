using System;
using System.Web.Http;
using Seemplest.Core.ServiceObjects.Validation;
using SeemplestCloud.WebClient.Infrastructure;

namespace SeemplestCloud.WebClient.Controllers
{
    /// <summary>
    /// This API manages operations related to subscriptions
    /// </summary>
    [RoutePrefix("api/subscription")]
    public class SubscriptionApiController: ApiController
    {
        [Route("getresult/{id}")]
        public int GetResult(int id)
        {
            throw new ArgumentNullException("id");
        }

        [Route("getresult2/{id}")]
        public int GetResult2(int id)
        {
            throw new ArgumentValidationException().AddOperationError("Operation");
        }

        [Route("getmessage")]
        public string GetMessage()
        {
            return "Hello!";
        }
    }
}