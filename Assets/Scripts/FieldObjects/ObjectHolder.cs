using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ClashTheCube
{
    [RequireComponent(typeof(IMetaSerializable))]
    public class ObjectHolder : MonoBehaviour, IFieldObjectHolder
    {
        [SerializeField] private CubeController cubePrefab;
        [SerializeField] private BombController bombPrefab;
        
        private readonly List<FieldObjectBase> objects = new List<FieldObjectBase>();
        private IMetaSerializable serializer;

        private void Awake()
        {
            serializer = GetComponent<IMetaSerializable>();
        }

        public IEnumerable<FieldObjectBase> GetObjects()
        {
            return objects;
        }

        public IEnumerable<CubeController> GetCubes()
        {
            return objects.OfType<CubeController>();
        }

        public IEnumerable<BombController> GetBombs()
        {
            return objects.OfType<BombController>();
        }

        public void AddObject(FieldObjectBase obj)
        {
            if (objects.Contains(obj))
            {
                return;
            }
            
            objects.Add(obj);
        }

        public void RemoveObject(FieldObjectBase obj)
        {
            if (!objects.Contains(obj))
            {
                return;
            }

            objects.Remove(obj);
            Destroy(obj.gameObject);
        }
        
        public CubeController CreateCube()
        {
            return CreateCube(transform.position, Quaternion.identity);
        }

        public CubeController CreateCube(Vector3 position, Quaternion quaternion)
        {
            var cube = Instantiate(cubePrefab, position, quaternion);
            cube.SetSerializer(serializer);
            cube.SetObjectHolder(this);

            return cube;
        }

        public BombController CreateBomb()
        {
            return CreateBomb(transform.position, Quaternion.identity);
        }

        public BombController CreateBomb(Vector3 position, Quaternion quaternion)
        {
            var bomb = Instantiate(bombPrefab, position, quaternion);
            bomb.SetObjectHolder(this);
            
            return bomb;
        }
    }
}