using System;
using System.Collections.Generic;
using Databox;
using UnityEngine;
using Framework.Variables;
using Framework.Events;
using Framework.Utils;
using Framework.SystemInfo;
using DG.Tweening;
using TMPro;

namespace ClashTheCube
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Renderer))]
    public class CubeController : FieldObjectBase
    {
        [SerializeField] private FloatReference backZVelocityThreshold;
        [SerializeField] private GameEvent cubeMergeEvent;
        [SerializeField] private GameEvent cubeCrossedRedLineEvent;
        [SerializeField] private GameEvent cubeMetaSavedEvent;

        [SerializeField] private TextMeshPro[] labels;

        [SerializeField] private Color[] colors;

        [SerializeField] private IntVariable nextCubeNumber;
        [SerializeField] private Vector3Variable nextCubePosition;

        private BoxCollider boxCollider;
        private Renderer boxRenderer;
        private MaterialPropertyBlock materialBlock;

        public int Number { get; private set; }

        private int identifier;
        private int redLineCrossCount;
        private bool redLineHitActive;
        private static readonly int TintColorA = Shader.PropertyToID("_TintColorA");

        private IMetaSerializable serializer;

        public void SetSerializer(IMetaSerializable newSerializer)
        {
            if (newSerializer == null)
            {
                return;
            }
            
            serializer = newSerializer;
        }

        private new void Awake()
        {
            base.Awake();

            boxRenderer = GetComponent<Renderer>();
            boxCollider = GetComponent<BoxCollider>();
            materialBlock = new MaterialPropertyBlock();

            identifier = GetInstanceID();
            redLineHitActive = false;
        }

        private void Start()
        {
            destPosition = transform.position;
        }

        private new void FixedUpdate()
        {
            if (Body.velocity.z < backZVelocityThreshold)
            {
                var objectVelocity = Body.velocity;
                Body.velocity = new Vector3(objectVelocity.x, objectVelocity.y, backZVelocityThreshold);
            }

            if (CheckSleepingChanged())
            {
                SaveSnapshot();
                CheckRedLine();
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!collider.gameObject.CompareTag("Red Line"))
            {
                return;
            }

            redLineHitActive = true;
            redLineCrossCount++;

            CheckRedLine();
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!collider.gameObject.CompareTag("Red Line"))
            {
                return;
            }

            redLineHitActive = false;
        }

        private bool CheckSleepingChanged()
        {
            var sleepingValue = Body.velocity.magnitude < 0.1f;
            if (sleeping == sleepingValue)
            {
                return false;
            }

            sleeping = sleepingValue;
            return true;
        }
        
        private void CheckRedLine()
        {
            if (redLineCrossCount <= 1 && (!sleeping || !redLineHitActive)) return;
            if (cubeCrossedRedLineEvent)
            {
                cubeCrossedRedLineEvent.Raise();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Platform"))
            {
                return;
            }
            if (State == FieldObjectState.Final)
            {
                return;
            }

            Body.constraints = RigidbodyConstraints.None;

            if (collision.gameObject.CompareTag("Cube"))
            {
                var cube = collision.gameObject.GetComponent<CubeController>();
                if (cube.State == FieldObjectState.Final)
                {
                    return;
                }

                CheckCollision(cube);
            }
        }

        private void CheckCollision(CubeController cube)
        {
            if (cube == null)
            {
                return;
            }

            if (cube.Number == Number)
            {
                cube.SetFinalState();
                SetFinalState();

                nextCubeNumber.SetValue(Number * 2);
                nextCubePosition.SetValue(Vector.Midpoint(transform.position, cube.transform.position));

                if (cubeMergeEvent)
                {
                    cubeMergeEvent.Raise();
                }
            }
        }

        public void Discharge()
        {
            State = FieldObjectState.Final;

            transform.localScale = Vector3.one;
            transform.DOScale(0f, 0.5f)
                     .SetEase(Ease.OutQuad)
                     .OnComplete(SetFinalState)
                     .Play();
        }

        public void InitNew(int number)
        {
            Init(number, FieldObjectState.Initial);

            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad).Play();
        }

        public void InitMerged(int number)
        {
            Init(number, FieldObjectState.Transition);
        }

        private void Init(int number, FieldObjectState state)
        {
            switch (state)
            {
                case FieldObjectState.Initial:
                    State = FieldObjectState.Initial;
                    Body.constraints = RigidbodyConstraints.FreezeRotation;
                    Body.isKinematic = true;
                    redLineCrossCount = 0;
                    break;

                case FieldObjectState.Transition:
                    State = FieldObjectState.Transition;
                    Body.constraints = RigidbodyConstraints.None;
                    Body.isKinematic = false;
                    redLineCrossCount = 1;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, "Unable to init Cube with this value of state");
            }

            SetNumber(number);
        }

        private void SetNumber(int number)
        {
            Number = number;

            var numberString = number.ToString();
            if (number > 8192)
            {
                numberString = (number / 1000) + "K";
            }

            foreach (var label in labels)
            {
                label.text = numberString;
            }

            var converted = System.Convert.ToString(number, 2);
            var index = converted.Length - 1;
            if (index > colors.Length)
            {
                index -= (index / colors.Length) * colors.Length;
            }

            materialBlock.SetColor(TintColorA, colors[index - 1]);
            boxRenderer.SetPropertyBlock(materialBlock);
        }

        public void SetFinalState()
        {
            State = FieldObjectState.Final;

            boxCollider.enabled = false;
            Body.velocity = Vector3.zero;

            RemoveSnapshot();
            Destroy(gameObject);
        }

        #region Database

        private void SaveSnapshot()
        {
            var objectTransform = transform;
            var snapshot = new MetaSnapshot(
                DataBaseController.Cubes_Table,
                identifier,
                new Dictionary<string, DataboxType>()
                {
                    {DataBaseController.Cubes_StateField, new IntType((int) State)},
                    {DataBaseController.Cubes_NumberField, new IntType(Number)},
                    {DataBaseController.Cubes_PositionField, new Vector3Type(objectTransform.position)},
                    {DataBaseController.Cubes_RotationField, new QuaternionType(objectTransform.rotation)}
                }
            );
            
            serializer.MetaSave(snapshot);
            
            if (cubeMetaSavedEvent)
            {
                cubeMetaSavedEvent.Raise();
            }
        }

        public void LoadSnapshot(string key)
        {
            identifier = int.Parse(key);
            sleeping = true;
            
            var snapshot = serializer.MetaLoad(DataBaseController.Cubes_Table, identifier);
            if (snapshot == null)
            {
                return;
            }

            IntType state = (IntType) snapshot.Fields[DataBaseController.Cubes_StateField];
            IntType num = (IntType) snapshot.Fields[DataBaseController.Cubes_NumberField];
            Vector3Type pos = (Vector3Type) snapshot.Fields[DataBaseController.Cubes_PositionField];
            QuaternionType rot = (QuaternionType) snapshot.Fields[DataBaseController.Cubes_RotationField];
            
            Init(num.Value, (FieldObjectState)state.Value);
            transform.position = pos.Value;
            transform.rotation = rot.Value;
        }

        private void RemoveSnapshot()
        {
            serializer.MetaRemove(
                DataBaseController.Cubes_Table,
                identifier
            );
        }

        #endregion
    }
}
