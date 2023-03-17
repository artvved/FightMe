using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class CasterStats : ScriptableObject
    {
        [Header("Chain Lightning")]
        public float ChainLightningCD;
        public int ChainLightningDamage;
        public int ChainLightningTargetsNumber;
        public float ChainLightningTargetsRadius;
        [Header("Mind Control")]
        public float MindControlCD;
        public float MindControlDuration;
        public int MindControlTargetsNumber;
        [Header("Meteor")]
        public float MeteorCD;
        public int MeteorDamage;
        public float MeteorAdditionalDamageMultiplier;
        public float MeteorTargetsRadius;
        [Header("Bloodlust")]
        public float LifestealPercent;
       
    }
}