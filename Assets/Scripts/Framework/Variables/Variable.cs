using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Variables
{
    public class Variable<T> : ScriptableObject
    {
    #if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
    #endif
        public T Value;

        public void SetValue(T value)
        {
            Value = value;
        }

        public void SetValue(Variable<T> value)
        {
            Value = value.Value;
        }
    }
}
