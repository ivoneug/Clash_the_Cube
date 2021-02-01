using UnityEngine;
using Framework.Variables;
using Framework.Events;
using Framework.Utils;
using DG.Tweening;
using TMPro;
using Databox;

public class CubeController : MonoBehaviour
{
    public enum CubeState
    {
        Initial,
        Transition,
        Final
    }

    [SerializeField] private Vector2Variable swipeDelta;
    [SerializeField] private FloatReference swipeDeltaMultiplier;
    [SerializeField] private FloatReference xConstraint;
    [SerializeField] private FloatReference velocity;
    [SerializeField] private FloatReference force;
    [SerializeField] private GameEvent cubeMergeEvent;

    [SerializeField] private TextMeshPro[] labels;

    [SerializeField] private Color[] colors;

    [SerializeField] private IntVariable nextCubeNumber;
    [SerializeField] private Vector3Variable nextCubePosition;

    public CubeState State { get; private set; }
    public Rigidbody Body { get; private set; }
    private BoxCollider boxCollider;
    private Renderer boxRenderer;
    private Vector3 destPosition;

    public int Number { get; private set; }

    [SerializeField] private DataboxObject databox;

    public static string Table = "Field";
    private static string StateField = "StateField";
    private static string NumberField = "Number";
    private static string PositionField = "PositionField";
    private static string RotationField = "RotationField";
    private int _identifier;
    private bool _sleeping;

    private void Awake()
    {
        boxRenderer = GetComponent<Renderer>();
        boxCollider = GetComponent<BoxCollider>();
        Body = GetComponent<Rigidbody>();

        _identifier = GetInstanceID();
        _sleeping = true;
    }

    private void Start()
    {
        destPosition = transform.position;
    }

    private void Update()
    {
        if (State != CubeState.Initial)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, destPosition, velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        UpdateMeta();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            return;
        }
        if (State == CubeState.Final)
        {
            return;
        }

        Body.constraints = RigidbodyConstraints.None;

        if (collision.gameObject.tag == "Cube")
        {
            var cube = collision.gameObject.GetComponent<CubeController>();
            if (cube.State == CubeState.Final)
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

    public void InitNew(int number)
    {
        Init(number, CubeState.Initial);

        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutQuad).Play();
    }

    public void InitMerged(int number)
    {
        Init(number, CubeState.Transition);
    }

    private void Init(int number, CubeState state)
    {
        switch (state)
        {
            case CubeState.Initial:
                State = CubeState.Initial;
                Body.constraints = RigidbodyConstraints.FreezeRotation;
                break;

            case CubeState.Transition:
                State = CubeState.Transition;
                Body.constraints = RigidbodyConstraints.None;
                break;
        }

        SetNumber(number);
    }

    private void SetNumber(int number)
    {
        Number = number;

        string numberString = number.ToString();
        if (number > 8192)
        {
            numberString = (number / 1000).ToString() + "K";
        }

        foreach (var label in labels)
        {
            label.text = numberString;
        }

        string converted = System.Convert.ToString(number, 2);
        int index = converted.Length - 1;
        if (index > colors.Length)
        {
            index -= (index / colors.Length) * colors.Length;
        }
        boxRenderer.material.SetColor("_TintColorA", colors[index - 1]);
    }

    public void SetFinalState()
    {
        State = CubeState.Final;

        boxCollider.enabled = false;
        Body.velocity = Vector3.zero;

        MetaRemove();
        Destroy(gameObject);
    }

    public void MoveLeft()
    {
        destPosition += new Vector3(swipeDelta.Value.x * swipeDeltaMultiplier, 0f, 0f);
        if (destPosition.x < -xConstraint)
        {
            destPosition = new Vector3(-xConstraint, destPosition.y, destPosition.z);
        }
    }

    public void MoveRight()
    {
        destPosition += new Vector3(swipeDelta.Value.x * swipeDeltaMultiplier, 0f, 0f);
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
        Body.AddForce(Vector3.forward * force);

        State = CubeState.Transition;
    }

    #region Database

    private void UpdateMeta()
    {
        bool sleeping = Body.velocity.magnitude < 0.1f;
        if (_sleeping == sleeping)
        {
            return;
        }

        _sleeping = sleeping;
        MetaSave();
    }

    private void MetaSave()
    {
        IntType state = new IntType((int)State);
        IntType num = new IntType(Number);
        Vector3Type pos = new Vector3Type(transform.position);
        QuaternionType rot = new QuaternionType(transform.rotation);

        if (!databox.EntryExists(Table, _identifier.ToString()))
        {
            databox.AddData(Table, _identifier.ToString(), StateField, state);
            databox.AddData(Table, _identifier.ToString(), NumberField, num);
            databox.AddData(Table, _identifier.ToString(), PositionField, pos);
            databox.AddData(Table, _identifier.ToString(), RotationField, rot);
        }
        else
        {
            databox.SetData<IntType>(Table, _identifier.ToString(), StateField, state);
            databox.SetData<IntType>(Table, _identifier.ToString(), NumberField, num);
            databox.SetData<Vector3Type>(Table, _identifier.ToString(), PositionField, pos);
            databox.SetData<QuaternionType>(Table, _identifier.ToString(), RotationField, rot);
        }

        databox.SaveDatabase();
    }

    public void MetaLoad(string key)
    {
        _identifier = int.Parse(key);
        _sleeping = true;

        if (!databox.EntryExists(Table, _identifier.ToString()))
        {
            return;
        }

        IntType state = databox.GetData<IntType>(Table, _identifier.ToString(), StateField);
        IntType num = databox.GetData<IntType>(Table, _identifier.ToString(), NumberField);
        Vector3Type pos = databox.GetData<Vector3Type>(Table, _identifier.ToString(), PositionField);
        QuaternionType rot = databox.GetData<QuaternionType>(Table, _identifier.ToString(), RotationField);

        Init(num.Value, (CubeState)state.Value);
        transform.position = pos.Value;
        transform.rotation = rot.Value;
    }

    private void MetaRemove()
    {
        databox.RemoveEntry(Table, _identifier.ToString());
    }

#endregion
}
