using System;

namespace Framework.Variables
{
    [Serializable]
    public class StringReference : Reference<string>
    {
        public StringVariable Variable;

        public string Value
        {
            get { return UseConstant ? ConstantValue : Variable.Value; }
        }

        public static implicit operator string(StringReference reference)
        {
            return reference.Value;
        }
    }
}