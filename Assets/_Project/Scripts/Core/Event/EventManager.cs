using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AE.Core.Event
{
    [UsedImplicitly]
    public class EventManager
    {
        private readonly Dictionary<Type, List<Delegate>> _eventSubscribers = new();

        public void Subscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (!_eventSubscribers.ContainsKey(eventType))
            {
                _eventSubscribers[eventType] = new List<Delegate>();
            }

            _eventSubscribers[eventType].Add(handler);
        }

        public void Unsubscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (!_eventSubscribers.ContainsKey(eventType))
                return;

            _eventSubscribers[eventType].Remove(handler);

            if (_eventSubscribers[eventType].Count == 0)
            {
                _eventSubscribers.Remove(eventType);
            }
        }

        public void Notify<TEvent>(TEvent eventData) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            if (!_eventSubscribers.ContainsKey(eventType))
                return;

            var handlers = _eventSubscribers[eventType];
            foreach (EventHandler<TEvent> handler in handlers)
            {
                handler(in eventData);
            }
        }
    }
}