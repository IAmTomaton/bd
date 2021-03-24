using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DLQ
{
    public interface DeadLetterQueueManager
    {
        DeadLetterQueue<T> CreateQueue<T>(string name);
        void DeleteQueue(string name);
    }

    public interface DeadLetterQueue<T>
    {
        void PutTask(T task);
    }

    public interface EventHandler<T> where T : Event
    {
        void Handle(T ev);
    }

    public interface EventSource<T> where T : Event
    {
        IList<T> Events(string lastEventId);
    }

    public interface StateStorage
    {
        void SaveState(String lastEventId);

        string GetState();
    }

    public class Event
    {
        public string Id { get; set; }
        public string EntityId { get; set; }
    }


    public class Daemon<T> where T : Event
    {
        private EventHandler<T> eventHandler;
        private EventSource<T> eventSource;
        private StateStorage stateStorage;
        private DeadLetterQueueManager dlqManager;

        private ConcurrentDictionary<string, DeadLetterQueue<T>> deadLetterQueues;

        public Daemon(EventHandler<T> eventHandler, EventSource<T> eventSource,
            StateStorage stateStorage, DeadLetterQueueManager dlqManager)
        {
            this.eventHandler = eventHandler;
            this.eventSource = eventSource;
            this.stateStorage = stateStorage;
            this.dlqManager = dlqManager;

            deadLetterQueues = new ConcurrentDictionary<string, DeadLetterQueue<T>>();
        }

        public void Run()
        {
            Task.Run(Work);
        }

        public void DlqWasCleared(string dlqName)
        {
            dlqManager.DeleteQueue(dlqName);
            deadLetterQueues.TryRemove(dlqName, out _);
        }

        private async void Work()
        {
            while (true)
            {
                while (deadLetterQueues.Count > 10)
                    await Task.Delay(100);
                var events = eventSource.Events(stateStorage.GetState());
                foreach (var currentEvent in events)
                {
                    HandleEvent(currentEvent);
                    stateStorage.SaveState(currentEvent.Id);
                }
            }
        }

        private void HandleEvent(T currentEvent)
        {
            var dlqName = "DLQ." + currentEvent.EntityId;
            if (deadLetterQueues.TryGetValue(dlqName, out var deadLetterQueue))
            {
                deadLetterQueue.PutTask(currentEvent);
                return;
            }

            try
            {
                eventHandler.Handle(currentEvent);
            }
            catch
            {
                var queue = dlqManager.CreateQueue<T>(dlqName);
                deadLetterQueues.TryAdd(dlqName, queue);
                queue.PutTask(currentEvent);
            }
        }
    }
}