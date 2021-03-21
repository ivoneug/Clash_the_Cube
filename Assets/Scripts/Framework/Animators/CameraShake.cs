using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Animators
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField][Range(0f, 5f)] private float shakeRange = 1f;
        [SerializeField][Range(0f, 3f)] private float shakeDuration = 1f;
        [SerializeField][Range(1, 50)] private int numberOfShakes = 10;

        private Vector3 _initialPosition;
        private bool isShaking = false;
        private float firstShakeTime;
        private float lastShakeTime;
        private float shakesDelta;

        private void Update()
        {
            if (!isShaking)
            {
                return;
            }

            float time = Time.time;
            if (time - lastShakeTime > shakesDelta)
            {

                if (time - firstShakeTime > shakeDuration)
                {
                    transform.position = _initialPosition;
                    isShaking = false;
                    return;
                }

                lastShakeTime = time;

                float x = Random.Range(-shakeRange, shakeRange);
                float y = Random.Range(-shakeRange, shakeRange);

                transform.position = new Vector3(_initialPosition.x + x, _initialPosition.y + y, _initialPosition.z);
            }
        }

        public void Shake()
        {
            if (isShaking)
            {
                return;
            }

            _initialPosition = transform.position;

            firstShakeTime = Time.time;
            lastShakeTime = 0;
            shakesDelta = shakeDuration / (float)numberOfShakes;
            isShaking = true;
        }
    }
}