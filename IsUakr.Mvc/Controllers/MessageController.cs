using IsUakr.Entities.Messages;
using IsUakr.MessageBroker;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IsUakr.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMqManager _mqManager;

        public MessageController(IMqManager mqManager)
        {
            _mqManager = mqManager;
        }

        [HttpPost]
        public async Task<IActionResult> Load([FromBody] HubMessage message)
        {
            if(ModelState.IsValid)
                _mqManager.PublishMessage(message.ToString());

            await Task.Delay(50);
            return Ok();
        }
    }
}