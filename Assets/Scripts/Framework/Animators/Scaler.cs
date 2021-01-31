using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Animators
{
    [DisallowMultipleComponent]
    public class Scaler : MonoBehaviour
    {
        [SerializeField]
        Vector3 scaleVector;

        [SerializeField]
        float period = 2f;
        float scaleFactor;

        Vector3 startingScale;

        void Start()
        {
            startingScale = transform.localScale;
        }

        void Update()
        {
            if (period <= Mathf.Epsilon)
            {
                return;
            }

            const float tau = Mathf.PI * 2f; // ~ 6.28
            float cycles = Time.time / period;

            float sinValue = Mathf.Sin(cycles * tau) / 2f;
            scaleFactor = sinValue + 0.5f;

            transform.localScale = startingScale + scaleVector * scaleFactor;
        }
    }
}