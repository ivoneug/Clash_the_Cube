using UnityEngine;

namespace Framework.Variables
{
    [CreateAssetMenu]
    public class FloatVariable : Variable<float>
    {
        public void ApplyChange(float amount)
        {
            Value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            Value += amount.Value;
        }
    }
}