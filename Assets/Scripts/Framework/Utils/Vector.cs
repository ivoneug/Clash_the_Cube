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
