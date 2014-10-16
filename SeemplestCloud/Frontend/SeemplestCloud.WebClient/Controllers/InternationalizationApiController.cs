using System.Collections.Generic;
using System.Threading;
using System.Web.Http;
using SeemplestBlocks.Core.Internationalization;
using SeemplestBlocks.Dto.Internationalization;
using SeemplestCloud.Services;

namespace SeemplestCloud.WebClient.Controllers
{
    /// <summary>
    /// This controller provides an API for internationalization
    /// </summary>
    [RoutePrefix("api/intln")]
    public class InternationalizationApiController : ResourceControllerBase
    {
        [Route("current")]
        public string GetCurrentCulture()
        {
            return Thread.CurrentThread.CurrentUICulture.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("servicemessages")]
        public List<ResourceStringDto> GetErrorMessages()
        {
            return GetResources(typeof(ServiceMessages));
        }
    }
}

