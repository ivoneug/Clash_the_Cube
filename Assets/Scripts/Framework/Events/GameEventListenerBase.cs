using UnityEngine;
using UnityEngine.Events;

namespace Framework.Events
{
    public abstract class GameEventListenerBase : MonoBehaviour
    {
        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent Response;

        public virtual void OnEventRaised(GameEvent gameEvent)
        {
        }
    }
}
