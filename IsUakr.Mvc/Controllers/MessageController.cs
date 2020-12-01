using IsUakr.Entities.Messages;
using IsUakr.MessageBroker;
using IsUakr.MessageHandler;
using IsUakr.MessageHandler.DAL;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace IsUakr.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMqManager _mqManager;
        private readonly ConnStrProvider _provider;
        private static List<Worker> workers = new List<Worker>();

        public MessageController(IMqManager mqManager, ConnStrProvider provider)
        {
            _mqManager = mqManager;
            _provider = provider;
        }

        [HttpPost]
        public IActionResult Load([FromBody] HubMessage message)
        {
            if(ModelState.IsValid)
            {
                var newQueue = _mqManager.PublishMessage(message.ToString());
                if(!string.IsNullOrEmpty(newQueue))
                {
                    var worker = new Worker(new ConnStrProvider(newQueue, _provider.DbConnStr, _provider.MqConnStr), 
                                            new MessageProcessor(new FlatDecisionMaker(_provider.DbConnStr)));
                    worker.Execute();
                    workers.Add(worker);
                }
            }

            return Ok();
        }
    }
}