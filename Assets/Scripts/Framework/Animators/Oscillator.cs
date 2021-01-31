using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Soundy;

namespace Framework.Animators
{
    [DisallowMultipleComponent]
    public class Oscillator : MonoBehaviour
    {
        [SerializeField]
        SoundyData sfxData;

        [SerializeField]
        Vector3 movementVector;

        [SerializeField]
        float period = 2f;
        float movementFactor;

        Vector3 startingPosition;
        List<float> history = new List<float>();

        void Start()
        {
            startingPosition = transform.position;
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
            movementFactor = sinValue + 0.5f;

            transform.position = startingPosition + movementVector * movementFactor;

            history.Add(Mathf.Abs(sinValue));
            if (history.Count > 3)
            {
                history.RemoveAt(0);
            }

            // play the sound
            if (history.Count == 3 && history[0] < history[1] && history[2] < history[1])
            {
                PlaySound();
            }
        }

        private void PlaySound()
        {
            // play the sound
            if (sfxData != null)
            {
                SoundyManager.Play(sfxData);
            }
        }
    }
}
