using System.Web.Http;

namespace VolumeSync
{
    [RoutePrefix("api/ping")]
    public class PingApiController : ApiController
    {
        [HttpGet]
        [Route]
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
