using System.Threading.Tasks;
using IsUark.Mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace IsUark.Mvc.Controllers
{
    [Route("api/meterhubdata")]
    public class MeterHubDataController : Controller
    {
        [HttpPost]
        public async Task<OkResult> Post([FromBody] MeterHubData data)
        {
            return Ok();
        }
    }
}