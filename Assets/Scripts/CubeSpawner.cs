using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;
using Framework.Utils;
using Databox;
using Framework.Events;

namespace ClashTheCube
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private DataboxObject databox;

        [SerializeField] private CubeController cubePrefab;
        [SerializeField] private BombController bombPrefab;

        [SerializeField] private FloatReference spawnTimeDelta;

        [SerializeField] private IntReference maxPowNumberForCube;
        [SerializeField] private IntReference nextCubeNumber;
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

        [SerializeField] private IntReference nearestCubesCountToGenerateNumber;
        [SerializeField][Range(0f, 1f)] private float randomNumberGenerationChance = 0.25f;

        [SerializeField] private GameEvent achievementReachedEvent;
        [SerializeField] private IntReference achievementNumber;
        [SerializeField] private IntReference achievementMinNumber;

        [SerializeField] private IntReference zPositionThresholdForForceContinue;

        private int previousGeneratedNumber = -1;

        public void LoadSavedFieldState()
        {
            previousGeneratedNumber = -1;

            if (MetaLoad())
            {
                return;
            }

            Spawn();
        }

        public bool MetaLoad()
        {
            bool hasInitial = false;

            var entries = databox.GetEntriesFromTable(DataBaseController.Cubes_Table);
            foreach (var entry in entries)
            {
                var cube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
                cube.MetaLoad(entry.Key);

                if (cube.State == CubeState.Initial)
                {
                    hasInitial = true;
                }
            }

            return hasInitial;
        }

        public void MetaClear()
        {
            databox.RemoveDatabaseTable(DataBaseController.Cubes_Table);
            databox.AddDatabaseTable(DataBaseController.Cubes_Table);
            databox.SaveDatabase();
        }

        public void Spawn()
        {
            CancelInvoke(nameof(_Spawn));

            Invoke(nameof(_Spawn), spawnTimeDelta);
        }

        private void _Spawn()
        {
            var cube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
            cube.InitNew(GenerateCubeNumber());
        }

        public void SpawnMerge()
        {
            CheckAchievement();

            // var quaternion = Quaternion.Euler(Random.Range(-cubeMergeAngle, cubeMergeAngle), 0f, Random.Range(-cubeMergeAngle, cubeMergeAngle));
            var quaternion = Quaternion.identity;
            // var force = Vector3.up;
            var force = Vector3.zero;
            var torque = GenerateNormalizedTorque();

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

            var cube = Instantiate(cubePrefab, nextCubePosition.Value, quaternion);
            cube.InitMerged(nextCubeNumber.Value);

            cube.Body.AddForce(force * Random.Range(cubeMergeForceMin, cubeMergeForceMax));
            cube.Body.AddTorque(torque * Random.Range(cubeMergeTorqueMin, cubeMergeTorqueMax), ForceMode.Impulse);
        }

        public void SpawnBomb()
        {
            CancelInvoke(nameof(_SpawnBomb));

            Invoke(nameof(_SpawnBomb), spawnTimeDelta);
        }

        private void _SpawnBomb()
        {
            Instantiate(bombPrefab, transform.position, Quaternion.identity);
        }

        public void ActivateSuperMagnete()
        {
            var map = new Dictionary<int, List<CubeController>>();
            var objects = GameObject.FindGameObjectsWithTag("Cube");

            foreach (var obj in objects)
            {
                var cube = obj.GetComponent<CubeController>();
                if (cube.State != CubeState.Transition)
                {
                    continue;
                }

                if (!map.ContainsKey(cube.Number))
                {
                    map[cube.Number] = new List<CubeController>();
                }
                map[cube.Number].Add(cube);
            }

            foreach (var pair in map)
            {
                if (pair.Value.Count <= 1)
                {
                    continue;
                }

                var transforms = new List<Vector3>();
                foreach (var cube in pair.Value)
                {
                    transforms.Add(cube.transform.position);
                }

                var midpoint = Vector.Midpoint(transforms.ToArray());

                foreach (var cube in pair.Value)
                {
                    var torque = GenerateNormalizedTorque();

                    var direction = (midpoint - cube.transform.position).normalized;
                    var force = direction + Vector3.up * cubeMergeUpForceMultiplier;
                    
                    cube.Body.AddForce(force * Random.Range(cubeMergeForceMin, cubeMergeForceMax));
                    cube.Body.AddTorque(torque * Random.Range(cubeMergeTorqueMin, cubeMergeTorqueMax), ForceMode.Impulse);
                }
            }
        }

        public void DischargeActiveCube()
        {
            var objects = GameObject.FindGameObjectsWithTag("Cube");

            foreach (var obj in objects)
            {
                var cube = obj.GetComponent<CubeController>();
                if (cube.State != CubeState.Initial)
                {
                    continue;
                }

                cube.Discharge();
            }
        }

        private Vector3 GenerateNormalizedTorque()
        {
            var randomX = Random.Range(-cubeMergeAngle, cubeMergeAngle);
            var randomZ = Random.Range(-cubeMergeAngle, cubeMergeAngle);

            return -(new Vector3(randomX, 0f, randomZ)).normalized;
        }

        private CubeController GetNearestMatchingCube()
        {
            var objects = GameObject.FindGameObjectsWithTag("Cube");
            CubeController nearest = null;

            foreach (var obj in objects)
            {
                var cube = obj.GetComponent<CubeController>();
                if (cube.Number != nextCubeNumber.Value ||
                    cube.State != CubeState.Transition)
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

        private int GenerateCubeNumber()
        {
            int random = -1;
            do
            {
                random = (int)Mathf.Pow(2, Random.Range(1, maxPowNumberForCube + 1));
            } while (random == previousGeneratedNumber);

            if (Random.Range(0f, 1f) <= randomNumberGenerationChance || nearestCubesCountToGenerateNumber == 0)
            {
                // Debug.Log("Random Chance, generated: " + random);

                previousGeneratedNumber = random;
                return random;
            }

            var maxNumber = (int)Mathf.Pow(2, maxPowNumberForCube);

            // get all cubes and filter out all cubes with inappropreate numbers
            var objs = GameObject.FindGameObjectsWithTag("Cube");
            var objects = new List<CubeController>();

            foreach (var obj in objs)
            {
                var cube = obj.GetComponent<CubeController>();
                if (cube.Number > maxNumber || cube.State != CubeState.Transition)
                {
                    continue;
                }

                objects.Add(cube);
            }

            // check that we have enough cubes
            if (objects.Count < nearestCubesCountToGenerateNumber)
            {
                // Debug.Log("Not enough cubes, generated: " + random);

                previousGeneratedNumber = random;
                return random;
            }

            var position = transform.position;

            // sort all cubes by distance to spawner
            objects.Sort((CubeController x, CubeController y) =>
            {
                var xDistance = Vector.DistanceTo(position, x.transform.position);
                var yDistance = Vector.DistanceTo(position, y.transform.position);

                return xDistance < yDistance ? -1 : 1;
            });

            // remove all extra cubes
            if (objects.Count > nearestCubesCountToGenerateNumber)
            {
                objects.RemoveRange(nearestCubesCountToGenerateNumber, objects.Count - nearestCubesCountToGenerateNumber);
            }

            Shuffle.List<CubeController>(objects);
            // bool found = false;
            foreach (var cube in objects)
            {
                if (cube.Number != previousGeneratedNumber)
                {
                    random = cube.Number;
                    // found = true;
                    break;
                }
            }

            // if (found)
            // {
            //     Debug.Log("Cubes count: " + objects.Count + ", generated: " + random);
            // }
            // else
            // {
            //     Debug.Log("Cubes count: " + objects.Count + ", but all the same as previous, generated: " + random);
            // }

            previousGeneratedNumber = random;
            return random;
        }

        private void CheckAchievement()
        {
            if (nextCubeNumber < achievementMinNumber || achievementReachedEvent == null)
            {
                return;
            }

            achievementNumber.Variable.Value = nextCubeNumber;
            achievementReachedEvent.Raise();
        }

        public void ForceContinueGame()
        {
            var objects = GameObject.FindGameObjectsWithTag("Cube");

            foreach (var obj in objects)
            {
                var cube = obj.GetComponent<CubeController>();
                if (cube.State != CubeState.Transition)
                {
                    continue;
                }

                // remove all cubes which have Z < zPositionThresholdForForceContinue
                if (cube.transform.position.z < zPositionThresholdForForceContinue)
                {
                    cube.SetFinalState();
                }
            }
        }
    }
}
