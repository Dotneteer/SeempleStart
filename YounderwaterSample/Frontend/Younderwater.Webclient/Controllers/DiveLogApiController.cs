using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SeemplestBlocks.Core.ServiceInfrastructure;
using Younderwater.Dto.DiveLog;
using Younderwater.Services.DiveLog;

namespace Younderwater.Webclient.Controllers
{
    [Authorize]
    [RoutePrefix("diveLog")]
    public class DiveLogApiController : ApiController
    {
        /// <summary>
        /// A bejelentkezett felhasználó összes merülését lekérdezi
        /// </summary>
        /// <returns>Az összes regisztrált merülés</returns>
        [HttpGet]
        [Route("")]
        public async Task<List<DiveLogEntryDto>> GetAllDivesOfUserAsync()
        {
            var srvObj = HttpServiceFactory.CreateService<IDiveLogService>();
            return await srvObj.GetAllDivesOfUserAsync();
        }

        /// <summary>
        /// Lekérdezi az adott azonosítójú merülés részleteit
        /// </summary>
        /// <param name="diveId">A merülés azonosítója</param>
        /// <returns>A merülés adatai</returns>
        [HttpGet]
        [Route("{diveId}")]
        public async Task<DiveLogEntryDto> GetDiveByIdAsync(int diveId)
        {
            var srvObj = HttpServiceFactory.CreateService<IDiveLogService>();
            return await srvObj.GetDiveByIdAsync(diveId);
        }

        /// <summary>
        /// Regisztrálja az átadott merülést
        /// </summary>
        /// <param name="dive">A merülés adatai</param>
        /// <returns>Az újonnan regisztrált merülés adatai</returns>
        [HttpPost]
        [Route("")]
        public async Task<int> RegisterDiveLogEntryAsync(DiveLogEntryDto dive)
        {
            var srvObj = HttpServiceFactory.CreateService<IDiveLogService>();
            return await srvObj.RegisterDiveLogEntryAsync(dive);
        }

        /// <summary>
        /// Módosítja az átadott merülés részleteit
        /// </summary>
        /// <param name="dive">A merülés adatai</param>
        [HttpPut]
        [Route("")]
        public async Task ModifyDiveLogEntryAsync(DiveLogEntryDto dive)
        {
            var srvObj = HttpServiceFactory.CreateService<IDiveLogService>();
            await srvObj.ModifyDiveLogEntryAsync(dive);
        }

        /// <summary>
        /// Eltávolítja az adott merülés adatait.
        /// </summary>
        /// <param name="diveId">A merülés azonosítója</param>
        [HttpDelete]
        [Route("{diveId}")]
        public async Task RemoveDiveLogEntryAsync(int diveId)
        {
            var srvObj = HttpServiceFactory.CreateService<IDiveLogService>();
            await srvObj.RemoveDiveLogEntryAsync(diveId);
        }
    }
}
