using System;

namespace Framework.Variables
{
    [Serializable]
    public class FloatReference : Reference<float>
    {
        public FloatVariable Variable;

        public float Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator float(FloatReference reference)
        {
            return reference.Value;
        }
    }
}