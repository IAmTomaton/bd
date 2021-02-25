using System;
using System.Collections.Generic;
using System.Threading;

namespace DLQ
{
    interface DeadLetterQueueManager
    {
        DeadLetterQueue<T> CreateQueue<T>(string name);
        void DeleteQueue(string name);
    }

    interface DeadLetterQueue<T>
    {
        void PutTask(T task);
    }

    interface EventHandler<T> where T : Event
    {
        void Handle(T ev);
    }

    interface EventSource<T> where T : Event
    {
        IList<T> Events(string lastEventId);
    }

    interface StateStorage
    {
        void SaveState(String lastEventId);

        string GetState();
    }

    class Event
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
    }


    class Daemon<T> where T : Event
    {
        private EventHandler<T> eventHandler;
        private EventSource<T> eventSource;
        private StateStorage stateStorage;
        private DeadLetterQueueManager dlqManager;

        private Dictionary<string, DeadLetterQueue<T>> deadLetterQueues;
        private Thread thread;
        private bool shouldWork;

        public Daemon(EventHandler<T> eventHandler, EventSource<T> eventSource,
            StateStorage stateStorage, DeadLetterQueueManager dlqManager)
        {
            this.eventHandler = eventHandler;
            this.eventSource = eventSource;
            this.stateStorage = stateStorage;
            this.dlqManager = dlqManager;

            deadLetterQueues = new Dictionary<string, DeadLetterQueue<T>>();
        }

        public void Run()
        {
            shouldWork = true;
            thread = new Thread(Work);
            thread.Start();
        }

        public void DlqWasCleared(string dlqName)
        {
            deadLetterQueues.Remove(dlqName);
            dlqManager.DeleteQueue(dlqName);
        }

        private void Work()
        {
            while (shouldWork)
            {
                var events = eventSource.Events(stateStorage.GetState());
                foreach (var currentEvent in events)
                {
                    while (deadLetterQueues.Count > 10)
                        continue;

                    HandleEvent(currentEvent);

                    stateStorage.SaveState(currentEvent.Id);
                }
            }
        }

        private void HandleEvent(T currentEvent)
        {
            var dlqName = "DLQ." + currentEvent.EntityId;
            Console.WriteLine(dlqName);
            if (deadLetterQueues.ContainsKey(dlqName))
            {
                deadLetterQueues[dlqName].PutTask(currentEvent);
                return;
            }

            try
            {
                eventHandler.Handle(currentEvent);
            }
            catch
            {
                var queue = dlqManager.CreateQueue<T>(dlqName);
                deadLetterQueues.Add(dlqName, queue);
                queue.PutTask(currentEvent);
            }
        }

        ~Daemon()
        {
            shouldWork = false;
        }
    }
}