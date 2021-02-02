using System;

namespace Framework.Variables
{
    [Serializable]
    public class IntReference : Reference<int>
    {
        public IntVariable Variable;

        public int Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator int(IntReference reference)
        {
            return reference.Value;
        }
    }
}