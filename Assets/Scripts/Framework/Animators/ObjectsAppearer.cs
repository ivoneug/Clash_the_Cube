using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Soundy;

namespace Framework.Animators
{
    [DisallowMultipleComponent]
    public class ObjectsAppearer : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] objects;

        [SerializeField]
        [Range(0f, 10f)]
        private float delayBetweenActivation = 0f;

        [SerializeField]
        private bool isTrigger = false;

        [Header("Activation Sound")]
        [SerializeField]
        private SoundyData activationSound;

        private bool activated = false;

        void Start()
        {
            foreach (var item in objects)
            {
                item.SetActive(false);
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        void OnTriggerEnter(Collider collider)
        {
            if (objects.Length == 0) return;
            if (!isTrigger) return;
            if (collider.gameObject.tag != "Player") return;

            Activate();
        }

        public void Activate()
        {
            if (activated) return;
            activated = true;

            StartCoroutine(_Activate());
        }

        IEnumerator _Activate()
        {
            bool multiSound = delayBetweenActivation > 0f;

            if (!multiSound)
            {
                SoundyManager.Play(activationSound);
            }

            foreach (var item in objects)
            {
                item.SetActive(true);

                if (multiSound)
                {
                    SoundyManager.Play(activationSound);
                }

                yield return new WaitForSeconds(delayBetweenActivation);
            }
        }
    }
}