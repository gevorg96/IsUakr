using IsUakr.Entities.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsUakr.MessageHandler.DAL
{
    public class MessageProcessor: IProcessor
    {
        private readonly IDecisionMaker _decisionMaker;
        public MessageProcessor(IDecisionMaker decisionMaker)
        {
            _decisionMaker = decisionMaker;
        }

        public async Task<string> Process(string message)
        {
            HubMessage msg = null;
            Dictionary<MeterMessage, string> data = new Dictionary<MeterMessage, string>();

            try
            {
                msg = JsonConvert.DeserializeObject<HubMessage>(message);

            }
            catch (Exception ex)
            {
                return "Необработанное сообщение. " +
                    GetHubId(new string[] { "{", "," }, message.Split("Messages").First()) +
                    "\n. Exception: " + ex.Message;
            }

            await _decisionMaker.UsingConnectionAsync(async (conn) =>
            {
                foreach (var meterMessage in msg.Messages)
                {
                    if (meterMessage.Body.State == 1)
                    {
                        var table = await _decisionMaker.MakeDecision(conn, meterMessage.Id);
                        if (!string.IsNullOrEmpty(table))
                            data.Add(meterMessage, table);
                    }
                }

                var newData = data.OrderBy(p => p.Value).GroupBy(x => x.Value);

                var sb = new StringBuilder();
                foreach (var pairs in newData)
                {
                    sb.Append(CreateMiltiInsertQuery(pairs.Key, pairs));
                }

                await _decisionMaker.Insert(conn, sb.ToString());
            });
            return "Количество обработанных сообщений: " + data.Count;
        }

        private string CreateMiltiInsertQuery(string table, IEnumerable<KeyValuePair<MeterMessage, string>> list)
        {
            var sb = new StringBuilder($"insert into public.{table} (meterdt, meterid, serial, volume, unit) VALUES");

            var res = list.Select(element => $"(to_timestamp('{element.Key.Body.MeterDt.Value.ToString()}', 'dd.mm.yyyy hh24:mi:ss'), " +
                    $"{element.Key.Id}, {element.Key.SerialNumber}, {element.Key.Body.Volume}, {element.Key.Body.MeasureUnit})");

            sb.Append(string.Join(',', res));
            sb.Append(";");
            return sb.ToString();
        }

        private string GetHubId(string[] parts, string mess)
        {
            foreach (var part in parts)
            {
                mess.Replace(part, "");
            }
            return mess;
        }
    }
}
