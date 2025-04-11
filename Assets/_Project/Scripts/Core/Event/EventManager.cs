using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace AE.Core.Event
{
    [UsedImplicitly]
    public class EventManager
    {
        private readonly Dictionary<Type, List<Delegate>> _eventSubscribers = new();

        public void Subscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : struct, IEvent
        {
            var eventType = typeof(TEvent);
            if (!_eventSubscribers.ContainsKey(eventType))
            {
                _eventSubscribers[eventType] = new List<Delegate>();
            }

            _eventSubscribers[eventType].Add(handler);
        }

        public void Unsubscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : struct, IEvent
        {
            var eventType = typeof(TEvent);
            if (!_eventSubscribers.TryGetValue(eventType, out var subscriber))
                return;

            subscriber.Remove(handler);

            if (_eventSubscribers[eventType].Count == 0)
            {
                _eventSubscribers.Remove(eventType);
            }
        }

        public void Notify<TEvent>(TEvent eventData) where TEvent : struct, IEvent
        {
            var eventType = typeof(TEvent);
            if (!_eventSubscribers.TryGetValue(eventType, out var handlers))
                return;

            foreach (var @delegate in handlers)
            {
                ((EventHandler<TEvent>)@delegate)(in eventData);
            }
        }
    }
}