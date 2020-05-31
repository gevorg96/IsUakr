using System.Threading.Tasks;
using IsUakr.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace IsUakr.Mvc.Controllers
{
    [Route("api/meterhubdata")]
    public class MeterHubDataController : Controller
    {
        [HttpPost]
        public OkResult Post([FromBody] MeterHubData data)
        {
            return Ok();
        }
    }
}