using UnityEngine;
using Framework.Variables;
using Framework.Utils;
using Databox;

namespace ClashTheCube
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private DataboxObject databox;

        [SerializeField] private GameObject cubePrefab;

        [SerializeField] private IntVariable nextCubeNumber;
        [SerializeField] private Vector3Variable nextCubePosition;
        [SerializeField] private FloatReference cubeMergeSideForceMultiplier;
        [SerializeField] private FloatReference cubeMergeUpForceMultiplier;
        [SerializeField] private FloatReference cubeMergeMinDistanceToApplyForce;
        [SerializeField] private FloatReference cubeMergeMaxBackDistanceToApplyForce;
        [SerializeField] private FloatReference cubeMergeForceMin;
        [SerializeField] private FloatReference cubeMergeForceMax;
        [SerializeField] private FloatReference cubeMergeAngle;
        [SerializeField] private FloatReference cubeMergeTorqueMin;
        [SerializeField] private FloatReference cubeMergeTorqueMax;

        private void Start()
        {
            if (MetaLoad())
            {
                return;
            }

            Spawn();
        }

        public bool MetaLoad()
        {
            bool hasInitial = false;

            var entries = databox.GetEntriesFromTable(CubeController.Table);
            foreach (var entry in entries)
            {
                var cube = Instantiate(cubePrefab, transform.position, Quaternion.identity).GetComponent<CubeController>();
                cube.MetaLoad(entry.Key);

                if (cube.State == CubeController.CubeState.Initial)
                {
                    hasInitial = true;
                }
            }

            return hasInitial;
        }

        public void Spawn()
        {
            CancelInvoke("_Spawn");

            Invoke("_Spawn", 0.7f);
        }

        private void _Spawn()
        {
            var cube = Instantiate(cubePrefab, transform.position, Quaternion.identity).GetComponent<CubeController>();
            cube.InitNew((int)Mathf.Pow(2, Random.Range(1, 7)));
            // cube.InitNew((int)Mathf.Pow(2, 12));
        }

        public void SpawnMerge()
        {
            // var quaternion = Quaternion.Euler(Random.Range(-cubeMergeAngle, cubeMergeAngle), 0f, Random.Range(-cubeMergeAngle, cubeMergeAngle));
            var quaternion = Quaternion.identity;
            // var force = Vector3.up;
            var force = Vector3.zero;
            var torque = -(new Vector3(Random.Range(-cubeMergeAngle, cubeMergeAngle), 0f, Random.Range(-cubeMergeAngle, cubeMergeAngle))).normalized;

            var nearestCube = GetNearestMatchingCube();
            if (nearestCube != null)
            {
                var direction = (nearestCube.transform.position - nextCubePosition.Value).normalized * cubeMergeSideForceMultiplier;
                force = direction + Vector3.up * cubeMergeUpForceMultiplier;
            }
            else
            {
                force = Vector3.up * cubeMergeUpForceMultiplier;
            }

            var cube = Instantiate(cubePrefab, nextCubePosition.Value, quaternion).GetComponent<CubeController>();
            cube.InitMerged(nextCubeNumber.Value);

            cube.Body.AddForce(force * Random.Range(cubeMergeForceMin, cubeMergeForceMax));
            // cube.Body.AddTorque(Vector.Vector3RandomNormal() * cubeMergeTorque, ForceMode.Impulse);
            cube.Body.AddTorque(torque * Random.Range(cubeMergeTorqueMin, cubeMergeTorqueMax), ForceMode.Impulse);
        }

        private CubeController GetNearestMatchingCube()
        {
            var objects = GameObject.FindGameObjectsWithTag("Cube");
            CubeController nearest = null;

            foreach (var obj in objects)
            {
                var cube = obj.GetComponent<CubeController>();
                if (cube.Number != nextCubeNumber.Value ||
                    cube.State != CubeController.CubeState.Transition)
                {
                    continue;
                }

                if (nearest == null)
                {
                    nearest = cube;
                }
                else
                {
                    var distance1 = Vector.DistanceTo(nextCubePosition.Value, nearest.transform.position);
                    var distance2 = Vector.DistanceTo(nextCubePosition.Value, cube.transform.position);

                    nearest = distance2 < distance1 ? cube : nearest;
                }
            }

            if (nearest != null)
            {
                var nearestPos = nearest.transform.position;

                if (Vector.DistanceTo(nextCubePosition.Value, nearestPos) < cubeMergeMinDistanceToApplyForce)
                {
                    nearest = null;
                }
                else if (nextCubePosition.Value.z > nearestPos.z &&
                         Vector.DistanceTo(nextCubePosition.Value, nearestPos) > cubeMergeMaxBackDistanceToApplyForce)
                {
                    nearest = null;
                }
            }

            return nearest;
        }
    }
}
