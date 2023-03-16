using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class UnitStats : ScriptableObject
    {
        public float Speed;
        public int MaxHp;
        public int Damage;
        public float Range;
        public float AttackRatio;
        public int Coins;
        public int HpRegenValue;
        public int HpRegenSpeed;
    }
}