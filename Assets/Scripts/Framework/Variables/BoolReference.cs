using System;

namespace Framework.Variables
{
    [Serializable]
    public class BoolReference : Reference<bool>
    {
        public BoolVariable Variable;

        public bool Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator bool(BoolReference reference)
        {
            return reference.Value;
        }
    }
}