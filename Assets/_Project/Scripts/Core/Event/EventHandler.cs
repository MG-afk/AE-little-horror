namespace AE.Core.Event
{
    public delegate void EventHandler<TEvent>(in TEvent eventData) where TEvent : IEvent;
}