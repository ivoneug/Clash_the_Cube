using System.Collections.Generic;
using UnityEngine;

namespace Framework.Events
{
    [CreateAssetMenu]
    public class GameEvent : ScriptableObject
    {
        [SerializeField]
        private bool active = true;

        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<GameEventListenerBase> eventListeners = new List<GameEventListenerBase>();

        public void Raise()
        {
            if (!active)
            {
                return;
            }

            try
            {
                for (int i = eventListeners.Count - 1; i >= 0; i--)
                {
                    eventListeners[i].OnEventRaised(this);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        public void RegisterListener(GameEventListenerBase listener)
        {
            if (!eventListeners.Contains(listener)) {
                eventListeners.Add(listener);
            }
        }

        public void UnregisterListener(GameEventListenerBase listener)
        {
            if (eventListeners.Contains(listener)) {
                eventListeners.Remove(listener);
            }
        }
    }
}