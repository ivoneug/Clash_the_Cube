using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Framework.Events;

public class InvokeAfterNumberOfAttempts : MonoBehaviour
{
    [SerializeField]
    private uint attempts;
    
    [SerializeField]
    private UnityEvent response;

    [SerializeField]
    private GameEvent gameEvent;

    private uint attemptsCounter = 0;

    public void TryInvoke()
    {
        attemptsCounter++;

        if (attemptsCounter >= attempts)
        {
            attemptsCounter = 0;

            response.Invoke();
            if (gameEvent)
            {
                gameEvent.Raise();
            }
        }
    }
}
