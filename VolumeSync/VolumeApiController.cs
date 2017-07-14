using System.Web.Http;

namespace VolumeSync
{
    [RoutePrefix("api/volume")]
    public class VolumeApiController : ApiController
    {
        private readonly VolumeManager _volumeManager = new VolumeManager();
        
        [HttpGet]
        [Route]
        public IHttpActionResult GetVolume()
        {
            return Ok(_volumeManager.GetVolume());
        }
        
        [HttpPut]
        [Route]
        public IHttpActionResult SetVolume([FromBody]VolumeModel volume)
        {
            _volumeManager.ChangeVolume(volume);
            return Ok();
        }
    }
}
