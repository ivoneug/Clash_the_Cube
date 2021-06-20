using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;
using Framework.SystemInfo;

namespace ClashTheCube
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class FieldObjectBase : MonoBehaviour
    {
        [SerializeField] protected Vector2Variable swipeDelta;
        [SerializeField] protected FloatReference swipeDeltaMultiplier;
        [SerializeField] protected FloatReference swipeDeltaMultiplierDesktop;

        [SerializeField] protected FloatReference xConstraint;
        [SerializeField] protected FloatReference force;
        [SerializeField] protected FloatReference velocity;

        [SerializeField] protected GameObject directionLine;

        public FieldObjectState State { get; protected set; }
        public Rigidbody Body { get; private set; }

        protected Vector3 destPosition;
        protected bool sleeping;
        protected IFieldObjectHolder objectHolder;

        public void SetObjectHolder(IFieldObjectHolder holder)
        {
            objectHolder = holder;
            objectHolder.AddObject(this);
        }
        
        protected void Awake()
        {
            Body = GetComponent<Rigidbody>();
            sleeping = true;
        }

        protected void Update()
        {
            UpdateDirectionLine();

            if (State != FieldObjectState.Initial)
            {
                return;
            }

            transform.position = Vector3.Lerp(transform.position, destPosition, velocity * Time.deltaTime);
        }

        protected void FixedUpdate()
        {
            sleeping = Body.velocity.magnitude < 0.1f;
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
            if (State != FieldObjectState.Initial)
            {
                return;
            }

            // accelerate here
            Body.isKinematic = false;
            Body.AddForce(Vector3.forward * force);

            State = FieldObjectState.Transition;
        }

        private void UpdateDirectionLine()
        {
            var active = State == FieldObjectState.Initial;

            if (directionLine.activeInHierarchy != active)
            {
                directionLine.SetActive(active);
            }
        }

        private float GetDeltaMultiplier()
        {
            return Platform.IsMobilePlatform() ? swipeDeltaMultiplier : swipeDeltaMultiplierDesktop;
        }
    }
}
