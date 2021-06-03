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
    [RequireComponent(typeof(Rigidbody))]
    public class BombController : MonoBehaviour
    {
        [SerializeField] private Vector2Variable swipeDelta;
        [SerializeField] private FloatReference swipeDeltaMultiplier;
        [SerializeField] private FloatReference swipeDeltaMultiplierDesktop;
        [SerializeField] private FloatReference xConstraint;
        [SerializeField] private FloatReference velocity;
        [SerializeField] private FloatReference force;
        [SerializeField] private GameObject directionLine;
        [SerializeField] private Vector3Variable explosionPosition;
        [SerializeField] private GameEvent bombExplosionEvent;

        public CubeState State { get; private set; }
        public Rigidbody Body { get; private set; }
        private BoxCollider boxCollider;
        private Vector3 destPosition;

        private bool sleeping;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            Body = GetComponent<Rigidbody>();

            sleeping = true;
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

        private void Update()
        {
            UpdateDirectionLine();

            if (State != CubeState.Initial)
            {
                return;
            }

            transform.position = Vector3.Lerp(transform.position, destPosition, velocity * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            sleeping = Body.velocity.magnitude < 0.1f;
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

        public void MoveLeft()
        {
            destPosition.x += swipeDelta.Value.x * GetDeltaMultiplier();
            if (destPosition.x < -xConstraint)
            {
                destPosition = new Vector3(-xConstraint, destPosition.y, destPosition.z);
            }
        }

        public void MoveRight()
        {
            destPosition.x += swipeDelta.Value.x * GetDeltaMultiplier();
            if (destPosition.x > xConstraint)
            {
                destPosition = new Vector3(xConstraint, destPosition.y, destPosition.z);
            }
        }

        public void Accelerate()
        {
            if (State != CubeState.Initial)
            {
                return;
            }

            // accelerate here
            Body.isKinematic = false;
            Body.AddForce(Vector3.forward * force);

            State = CubeState.Transition;
        }

        private float GetDeltaMultiplier()
        {
            return Platform.IsMobilePlatform() ? swipeDeltaMultiplier : swipeDeltaMultiplierDesktop;
        }

        private void UpdateDirectionLine()
        {
            var active = State == CubeState.Initial;

            if (directionLine.activeInHierarchy != active)
            {
                directionLine.SetActive(active);
            }
        }
    }
}
