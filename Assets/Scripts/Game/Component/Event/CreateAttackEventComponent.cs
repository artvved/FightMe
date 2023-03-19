using Leopotam.EcsLite;

namespace Game.Component
{
    public struct CreateAttackEventComponent
    {
        public EcsPackedEntity Sender;
        public EcsPackedEntity Target;
        public int Damage;
    }
}