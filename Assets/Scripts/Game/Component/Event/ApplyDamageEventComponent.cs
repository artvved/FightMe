using Leopotam.EcsLite;

namespace Game.Component
{
    public struct ApplyDamageEventComponent
    {
        public EcsPackedEntity Target;
        public int Damage;
    }
}