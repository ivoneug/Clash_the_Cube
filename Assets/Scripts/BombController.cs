using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;

using Framework.SystemInfo;
using Framework.Events;
using DG.Tweening;

namespace ClashTheCube
{
    [RequireComponent(typeof(BoxCollider))]
    public class BombController : FieldObjectBase
    {
        [SerializeField] private Vector3Variable explosionPosition;
        [SerializeField] private GameEvent bombExplosionEvent;

        private BoxCollider boxCollider;

        private new void Awake()
        {
            base.Awake();

            boxCollider = GetComponent<BoxCollider>();
        }

        private void Start()
        {
            destPosition = transform.position;

            State = CubeState.Initial;
            Body.constraints = RigidbodyConstraints.FreezeRotation;
            Body.isKinematic = true;

            var initialScale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(initialScale, 0.5f).SetEase(Ease.OutQuad).Play();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Platform"))
            {
                return;
            }
            if (State == CubeState.Final)
            {
                return;
            }

            if (collision.gameObject.CompareTag("Wall"))
            {
                Discharge();
            }

            if (collision.gameObject.CompareTag("Cube"))
            {
                var cube = collision.gameObject.GetComponent<CubeController>();
                if (cube.State == CubeState.Final)
                {
                    return;
                }

                cube.SetFinalState();
                Discharge();
            }
        }

        public void Discharge()
        {
            State = CubeState.Final;
            boxCollider.enabled = false;
            Body.velocity = Vector3.zero;

            transform.DOScale(0f, 0.5f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(() => Destroy(gameObject))
                     .Play();

            explosionPosition.SetValue(transform.position);
            
            if (bombExplosionEvent)
            {
                bombExplosionEvent.Raise();
            }
        }
    }
}
