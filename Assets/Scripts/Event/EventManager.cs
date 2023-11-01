namespace Events
{
    using System.Collections.Generic;
    using UnityEngine.Events;

    // This class manages events in the application.
    public class EventManager
    {
        // This class is a wrapper for UnityEvent that allows for type safety.
        private class InternalEvent<T> : UnityEvent<T>
        {
        }

        // This dictionary stores all the events in the application.
        private static readonly Dictionary<System.Type, object> eventDictionary = new Dictionary<System.Type, object>();

        // This method allows a listener to subscribe to an event of type T.
        public static void Subscribe<T>(UnityAction<T> listener)
        {
            object thisEvent;
            // If the event of type T exists in the dictionary, add the listener to it.
            if (eventDictionary.TryGetValue(typeof(T), out thisEvent))
            {
                ((UnityEvent<T>)thisEvent).AddListener(listener);
            }
            else
            {
                // If the event of type T does not exist in the dictionary, create a new event, add the listener to it, and add it to the dictionary.
                var ev = new InternalEvent<T>();
                ev.AddListener(listener);
                eventDictionary.Add(typeof(T), ev);
            }
        }

        // This method allows a listener to unsubscribe from an event of type T.
        public static void Unsubscribe<T>(UnityAction<T> listener)
        {
            object thisEvent;
            // If the event of type T exists in the dictionary, remove the listener from it.
            if (eventDictionary.TryGetValue(typeof(T), out thisEvent))
            {
                ((UnityEvent<T>)thisEvent).RemoveListener(listener);
            }
        }

        // This method broadcasts an event of type T to all its listeners.
        public static void Broadcast<T>(T context) where T : struct
        {
            object thisEvent;
            // If the event of type T exists in the dictionary, invoke it with the provided context.
            if (eventDictionary.TryGetValue(typeof(T), out thisEvent))
            {
                ((UnityEvent<T>)thisEvent).Invoke(context);
            }
        }

        // This method clears all the events from the dictionary.
        public static void CleanUp()
        {
            eventDictionary.Clear();
        }
    }
}