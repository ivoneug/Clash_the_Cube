using UnityEngine;
using Framework.Variables;
using Framework.Utils;

namespace ClashTheCube
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject cubePrefab;

        [SerializeField] private IntVariable nextCubeNumber;
        [SerializeField] private Vector3Variable nextCubePosition;
        [SerializeField] private FloatReference cubeMergeForceMin;
        [SerializeField] private FloatReference cubeMergeForceMax;
        [SerializeField] private FloatReference cubeMergeAngle;
        [SerializeField] private FloatReference cubeMergeTorque;

        private void Start()
        {
            Spawn();
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
            var quaternion = Quaternion.Euler(Random.Range(-cubeMergeAngle, cubeMergeAngle), 0f, Random.Range(-cubeMergeAngle, cubeMergeAngle));

            var cube = Instantiate(cubePrefab, nextCubePosition.Value, quaternion).GetComponent<CubeController>();
            cube.InitMerged(nextCubeNumber.Value);
            cube.Body.AddForce(Vector3.up * Random.Range(cubeMergeForceMin, cubeMergeForceMax));
            cube.Body.AddTorque(Vector.Vector3RandomNormal() * cubeMergeTorque, ForceMode.Impulse);
        }
    }
}
