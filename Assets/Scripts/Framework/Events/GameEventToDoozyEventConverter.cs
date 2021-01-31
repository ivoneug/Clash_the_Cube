using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine;

namespace Framework.Events
{
    public class GameEventToDoozyEventConverter : GameEventListenerBase
    {
        [Tooltip("Event to register with.")]
        public GameEvent Event;

        [Tooltip("Doozy event that should be send when Event occurs")]
        public string DoozyEvent;

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
            if (!string.IsNullOrEmpty(DoozyEvent))
            {
                StartCoroutine(SendGameEventInTheNextFrame());
            }
            
            Response.Invoke();
        }

        private IEnumerator SendGameEventInTheNextFrame()
        {
            yield return null;
            GameEventMessage.SendEvent(DoozyEvent);
        }
    }
}
