using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Utils
{
    public class Shuffle {
        public static void Array<T>(T[] arr) {
            for (int idx = 0; idx < arr.Length; idx++)
            {
                T tmp = arr[idx];
                int r = Random.Range(idx, arr.Length);

                arr[idx] = arr[r];
                arr[r] = tmp;
            }
        }

        public static void List<T>(List<T> list)
        {
            T[] arr = list.ToArray();
            list.Clear();
            Array<T>(arr);

            list.AddRange(arr);
        }
    }
}