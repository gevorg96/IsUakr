using System;
using System.Collections.Generic;
using System.Linq;

namespace IsUakr.MessageBroker.Commands
{
    public class Command
    {
        private KeyValuePair<WhatToDo, string> previousQueue;
        public List<KeyValuePair<WhatToDo, List<string>>> QueuesCommands { get; }

        public Command()
        {
            QueuesCommands = new List<KeyValuePair<WhatToDo, List<string>>>();
            QueuesCommands.Add(new KeyValuePair<WhatToDo, List<string>>(WhatToDo.Add, new List<string>()));
            QueuesCommands.Add(new KeyValuePair<WhatToDo, List<string>>(WhatToDo.Delete, new List<string>()));
            QueuesCommands.Add(new KeyValuePair<WhatToDo, List<string>>(WhatToDo.Nothing, new List<string>()));
        }

        public void Add(WhatToDo todo, string queueName)
        {
            QueuesCommands.First(x => x.Key == todo).Value.Add(queueName);
            previousQueue = new KeyValuePair<WhatToDo, string>(todo, queueName);
        }
    }

    public enum WhatToDo: short
    {
        Delete = 0,
        Add = 1,
        Nothing = 2
    }
}
