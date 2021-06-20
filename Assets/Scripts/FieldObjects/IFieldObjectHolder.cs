using System.Collections.Generic;
using UnityEngine;

namespace ClashTheCube
{
    public interface IFieldObjectHolder
    {
        IEnumerable<FieldObjectBase> GetObjects();
        IEnumerable<CubeController> GetCubes();
        IEnumerable<BombController> GetBombs();
        void AddObject(FieldObjectBase obj);
        void RemoveObject(FieldObjectBase obj);

        CubeController CreateCube();
        CubeController CreateCube(Vector3 position, Quaternion quaternion);

        BombController CreateBomb();
        BombController CreateBomb(Vector3 position, Quaternion quaternion);
    }
}