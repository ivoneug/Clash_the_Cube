using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Events;

public class GameEventRaiser : MonoBehaviour
{
    [Tooltip("Event to raise.")]
    public GameEvent Event;

    public void Raise()
    {
        if (!Event)
        {
            return;
        }

        Event.Raise();
    }
}
