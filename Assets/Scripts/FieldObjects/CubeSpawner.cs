using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Variables;
using Framework.Utils;
using Databox;
using Framework.Events;
using Random = UnityEngine.Random;

namespace ClashTheCube
{
    [RequireComponent(typeof(IFieldObjectHolder))]
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private DataboxObject databox;

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
        [SerializeField] [Range(0f, 1f)] private float randomNumberGenerationChance = 0.25f;

        [SerializeField] private GameEvent achievementReachedEvent;
        [SerializeField] private IntReference achievementNumber;
        [SerializeField] private IntReference achievementMinNumber;

        [SerializeField] private IntReference zPositionThresholdForForceContinue;

        private int previousGeneratedNumber = -1;
        private Coroutine spawnCubeRoutineHandle;
        private Coroutine spawnBombRoutineHandle;
        
        private IFieldObjectHolder objectHolder;

        private void Awake()
        {
            objectHolder = GetComponent<IFieldObjectHolder>();
        }

        public void LoadSavedFieldState()
        {
            previousGeneratedNumber = -1;

            if (MetaLoad())
            {
                return;
            }

            Spawn();
        }

        private bool MetaLoad()
        {
            var hasInitial = false;

            var entries = databox.GetEntriesFromTable(DataBaseController.Cubes_Table);
            foreach (var entry in entries)
            {
                var cube = objectHolder.CreateCube();
                cube.LoadSnapshot(entry.Key);

                if (cube.State == FieldObjectState.Initial)
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
            if (spawnCubeRoutineHandle != null)
            {
                StopCoroutine(spawnCubeRoutineHandle);
            }

            spawnCubeRoutineHandle = StartCoroutine(SpawnInternal());
        }

        private IEnumerator SpawnInternal()
        {
            yield return new WaitForSeconds(spawnTimeDelta);

            var cube = objectHolder.CreateCube();
            cube.InitNew(GenerateCubeNumber());
            spawnCubeRoutineHandle = null;
        }

        public void SpawnMerge()
        {
            CheckAchievement();

            var quaternion = Quaternion.identity;
            Vector3 force;
            var torque = GenerateNormalizedTorque();

            var nearestCube = GetNearestMatchingCube();
            if (nearestCube != null)
            {
                var direction = (nearestCube.transform.position - nextCubePosition.Value).normalized *
                                cubeMergeSideForceMultiplier;
                force = direction + Vector3.up * cubeMergeUpForceMultiplier;
            }
            else
            {
                force = Vector3.up * cubeMergeUpForceMultiplier;
            }

            var cube = objectHolder.CreateCube(nextCubePosition.Value, quaternion);
            cube.InitMerged(nextCubeNumber.Value);

            cube.Body.AddForce(force * Random.Range(cubeMergeForceMin, cubeMergeForceMax));
            cube.Body.AddTorque(torque * Random.Range(cubeMergeTorqueMin, cubeMergeTorqueMax), ForceMode.Impulse);
        }

        public void SpawnBomb()
        {
            if (spawnBombRoutineHandle != null)
            {
                StopCoroutine(spawnBombRoutineHandle);
            }

            spawnBombRoutineHandle = StartCoroutine(SpawnBombInternal());
        }

        private IEnumerator SpawnBombInternal()
        {
            yield return new WaitForSeconds(spawnTimeDelta);

            objectHolder.CreateBomb();
            spawnBombRoutineHandle = null;
        }

        public void ActivateSuperMagnete()
        {
            var map = new Dictionary<int, List<CubeController>>();
            var objects = objectHolder.GetCubes();

            foreach (var cube in objects)
            {
                if (cube.State != FieldObjectState.Transition)
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
                    cube.Body.AddTorque(torque * Random.Range(cubeMergeTorqueMin, cubeMergeTorqueMax),
                        ForceMode.Impulse);
                }
            }
        }

        public void DischargeActiveCube()
        {
            var objects = objectHolder.GetCubes();

            foreach (var cube in objects)
            {
                if (cube.State != FieldObjectState.Initial)
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
            var objects = objectHolder.GetCubes();
            CubeController nearest = null;

            foreach (var cube in objects)
            {
                if (cube.Number != nextCubeNumber.Value ||
                    cube.State != FieldObjectState.Transition)
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
            int random;
            do
            {
                random = (int) Mathf.Pow(2, Random.Range(1, maxPowNumberForCube + 1));
            } while (random == previousGeneratedNumber);

            if (Random.Range(0f, 1f) <= randomNumberGenerationChance || nearestCubesCountToGenerateNumber == 0)
            {
                previousGeneratedNumber = random;
                return random;
            }

            var maxNumber = (int) Mathf.Pow(2, maxPowNumberForCube);

            // get all cubes and filter out all cubes with inappropreate numbers
            var objs = objectHolder.GetCubes();
            var objects = new List<CubeController>();

            foreach (var cube in objs)
            {
                if (cube.Number > maxNumber || cube.State != FieldObjectState.Transition)
                {
                    continue;
                }

                objects.Add(cube);
            }

            // check that we have enough cubes
            if (objects.Count < nearestCubesCountToGenerateNumber)
            {
                previousGeneratedNumber = random;
                return random;
            }

            var position = transform.position;

            // sort all cubes by distance to spawner
            objects.Sort((x, y) =>
            {
                var xDistance = Vector.DistanceTo(position, x.transform.position);
                var yDistance = Vector.DistanceTo(position, y.transform.position);

                return xDistance < yDistance ? -1 : 1;
            });

            // remove all extra cubes
            if (objects.Count > nearestCubesCountToGenerateNumber)
            {
                objects.RemoveRange(nearestCubesCountToGenerateNumber,
                    objects.Count - nearestCubesCountToGenerateNumber);
            }

            Shuffle.List(objects);
            foreach (var cube in objects)
            {
                if (cube.Number == previousGeneratedNumber) continue;
                
                random = cube.Number;
                break;
            }

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
            var objects = objectHolder.GetCubes();

            foreach (var cube in objects)
            {
                if (cube.State != FieldObjectState.Transition)
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
