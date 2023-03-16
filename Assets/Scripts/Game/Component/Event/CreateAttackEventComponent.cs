using Leopotam.EcsLite;

namespace Game.Component
{
    public struct CreateAttackEventComponent
    {
        public EcsPackedEntity Creator;
        public EcsPackedEntity Target;
        public int Damage;
    }
}