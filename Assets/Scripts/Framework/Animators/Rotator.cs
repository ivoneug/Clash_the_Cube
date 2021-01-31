using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Animators
{
    [DisallowMultipleComponent]
    public class Rotator : MonoBehaviour
    {
        [SerializeField] Vector3 rotateVector;
        [SerializeField] float period = 2f;
        [SerializeField] bool isReversable = false;

        Quaternion startingRotation;
        float rotateFactor;

        void Start()
        {
            startingRotation = transform.rotation;
        }

        void Update()
        {
            if (period <= Mathf.Epsilon)
            {
                return;
            }

            if (isReversable)
            {
                const float tau = Mathf.PI * 2f; // ~ 6.28
                float cycles = Time.time / period;

                float sinValue = Mathf.Sin(cycles * tau) / 2f;
                rotateFactor = sinValue;// + 0.5f;

                transform.rotation = Quaternion.Euler(startingRotation.eulerAngles + rotateVector * rotateFactor);
                Debug.Log((rotateVector * rotateFactor).ToString());
            }
            else
            {
                transform.Rotate(rotateVector / period * Time.deltaTime);
            }
        }
    }
}