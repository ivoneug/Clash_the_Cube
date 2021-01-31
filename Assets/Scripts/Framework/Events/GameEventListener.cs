using UnityEngine;

namespace Framework.Events
{
    public class GameEventListener : GameEventListenerBase
    {
        [Tooltip("Event to register with.")]
        public GameEvent Event;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public override void OnEventRaised(GameEvent gameEvent)
        {
            Response.Invoke();
        }
    }
}