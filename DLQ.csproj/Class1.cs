using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DLQ
{
    class DeadLetterQueueManagerC : DeadLetterQueueManager
    {
        public DeadLetterQueue<T> CreateQueue<T>(string name)
        {
            return new DeadLetterQueueC<T>()
            {
                Name = name
            };
        }

        public void DeleteQueue(string name)
        {
            throw new NotImplementedException();
        }
    }

    class DeadLetterQueueC<T> : DeadLetterQueue<T>
    {
        public string Name { get; set; }

        public void PutTask(T task)
        {
            Console.WriteLine("Put task in " + Name);
        }
    }

    class EventHandlerC<T> : EventHandler<T> where T : Event
    {
        public void Handle(T ev)
        {
            
        }
    }

    class EventSourceC<T> : EventSource<T> where T : Event
    {
        public IList<T> Events(string lastEventId)
        {
            throw new NotImplementedException();
        }
    }

    class StateStorageC : StateStorage
    {
        public string GetState()
        {
            throw new NotImplementedException();
        }

        public void SaveState(string lastEventId)
        {
            throw new NotImplementedException();
        }
    }

    class EventF : Event
    {
        public Action Action { get; set; }

        public void Do()
        {
            Action();
        }
    }

    class Class1
    {
        public static void Do()
        {
            while (true)
            {
                var t = 0;
            }
        }

        public static void Main()
        {
            var d = new Daemon<>
        }
    }
}
