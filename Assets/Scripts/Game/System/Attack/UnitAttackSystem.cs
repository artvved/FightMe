using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class UnitAttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

       
        private readonly EcsPoolInject<ApplyDamageEventComponent> applyDamagePool = Startup.EVENT_WORLD;
        private readonly EcsPoolInject<CreateAttackEventComponent> createAttackPool = Startup.EVENT_WORLD;
        private readonly EcsPoolInject<PlayerTag> playerPool = default;
      
        private EcsFilter eventFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Startup.EVENT_WORLD);
            eventFilter = eventWorld.Filter<CreateAttackEventComponent>().End();
           
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                if (!createAttackPool.Value.Get(entity).Creator.Unpack(world,out int creator) || playerPool.Value.Has(creator))
                {
                    continue;
                }
                
                ref var applyDamageEventComponent = ref applyDamagePool.NewEntity(out int eventEnt);
                applyDamageEventComponent.Target = createAttackPool.Value.Get(entity).Target;
                applyDamageEventComponent.Damage = createAttackPool.Value.Get(entity).Damage;
            }
        }
    }
}