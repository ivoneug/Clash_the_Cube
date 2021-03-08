using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClashTheCube
{
    public enum AbilityType
    {
        None,
        SwitchCube,
        DropBomb,
        SuperMagnete
    }

    [CreateAssetMenu]
    public class AbilityTypeInfo : ScriptableObject
    {
        public AbilityType abilityType = AbilityType.None;
        public int defaultCount = 0;
        public int purchaseCount = 0;
        public string field;
        public string localNameKey;
        public string localInfoKey;
        public Sprite icon;

        public void CloneFrom(AbilityTypeInfo info)
        {
            this.abilityType = info.abilityType;
            this.defaultCount = info.defaultCount;
            this.purchaseCount = info.purchaseCount;
            this.field = info.field;
            this.localNameKey = info.localNameKey;
            this.localInfoKey = info.localInfoKey;
            this.icon = info.icon;
        }

        public override bool Equals(object info)
        {
            return Equals(info as AbilityTypeInfo);
        }

        public bool Equals(AbilityTypeInfo info)
        {
            return info != null &&
                   abilityType == info.abilityType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(AbilityTypeInfo x, AbilityTypeInfo y)
        {
            if (object.ReferenceEquals(x, y)) return true;
            if (object.ReferenceEquals(null, x)) return false;
            if (object.ReferenceEquals(null, y)) return false;

            return x.Equals(y);
        }

        public static bool operator !=(AbilityTypeInfo x, AbilityTypeInfo y)
        {
            return !(x == y);
        }
    }
}
