using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public class Vector
    {
        public static Vector3 Midpoint(Vector3 a, Vector3 b)
        {
            float x = a.x + (b.x - a.x) / 2f;
            float y = a.y + (b.y - a.y) / 2f;
            float z = a.z + (b.z - a.z) / 2f;

            return new Vector3(x, y, z);
        }

        public static Vector3 Midpoint(params Vector3[] arr)
        {
            if (arr.Length == 0)
            {
                return Vector3.zero;
            }
            else if (arr.Length == 1)
            {
                return arr[0];
            }

            List<Vector3> list = new List<Vector3>(arr);

            while (list.Count > 1)
            {
                var a = list[0];
                var b = list[1];
                var midpoint = Midpoint(a, b);

                list.RemoveRange(0, 2);
                list.Insert(0, midpoint);
            }

            return list[0];
        }

        public static Vector2 Midpoint(Vector2 a, Vector2 b)
        {
            float x = a.x + (b.x - a.x) / 2f;
            float y = a.y + (b.y - a.y) / 2f;

            return new Vector2(x, y);
        }

        public static float DistanceTo(Vector3 pos1, Vector3 pos2)
        {
            return Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2f) + Mathf.Pow(pos2.y - pos1.y, 2f) + Mathf.Pow(pos2.z - pos1.z, 2f));
        }

        public static float DistanceTo(Vector2 pos1, Vector2 pos2)
        {
            return Mathf.Sqrt(Mathf.Pow(pos2.x - pos1.x, 2f) + Mathf.Pow(pos2.y - pos1.y, 2f));
        }

        public static Vector3 Vector3RandomNormal()
        {
            return new Vector3(Random.Range(0f, 1.01f), Random.Range(0f, 1.01f), Random.Range(0f, 1.01f)).normalized;
        }

        public static Vector2 Vector2RandomNormal()
        {
            return new Vector2(Random.Range(0f, 1.01f), Random.Range(0f, 1.01f)).normalized;
        }
    }
}
