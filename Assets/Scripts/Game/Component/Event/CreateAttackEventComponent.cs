using Leopotam.EcsLite;

namespace Game.Component
{
    public struct CreateAttackEventComponent
    {
        public EcsPackedEntity Target;
        public int Damage;
    }
}