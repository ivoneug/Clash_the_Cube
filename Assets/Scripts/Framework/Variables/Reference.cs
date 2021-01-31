using System;

namespace Framework.Variables
{
    [Serializable]
    public class Reference<T>
    {
        public bool UseConstant = true;
        public T ConstantValue;

        public Reference()
        { }

        public Reference(T value)
        {
            UseConstant = true;
            ConstantValue = value;
        }
    }
}
