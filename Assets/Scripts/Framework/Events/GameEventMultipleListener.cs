using System.Collections.Generic;
using UnityEngine;

namespace Framework.Events
{
    public class GameEventMultipleListener : GameEventListenerBase
    {
        public enum MultipleListenerType
        {
            All,
            Any
        }

        [Tooltip("Listener type, for \"All\" response will be raised only when all event are fired, for \"Any\" - on any event.")]
        public MultipleListenerType type = MultipleListenerType.All;

        [Tooltip("Events to register with.")]
        public List<GameEvent> Events = new List<GameEvent>();

        private List<GameEvent> raisedEvents = new List<GameEvent>();

        private void OnEnable()
        {
            foreach (GameEvent gameEvent in Events)
            {
                gameEvent.RegisterListener(this);
            }
        }

        private void OnDisable()
        {
            foreach (GameEvent gameEvent in Events)
            {
                gameEvent.UnregisterListener(this);
            }
        }

        public override void OnEventRaised(GameEvent gameEvent)
        {
            if (type == MultipleListenerType.Any)
            {
                Response.Invoke();
                return;
            }

            if (!raisedEvents.Contains(gameEvent))
            {
                raisedEvents.Add(gameEvent);
            }

            if (raisedEvents.Count == Events.Count)
            {
                Response.Invoke();
                raisedEvents.Clear();
            }
        }
    }
}
