using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class ApplyDamageSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<UnitComponent> unitPool = default;
        private readonly EcsPoolInject<DeadTag> deadPool = default;
        private readonly EcsPoolInject<ApplyDamageEventComponent> applyDamagePool = Startup.EVENT_WORLD;

        private EcsFilter damageEventFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Startup.EVENT_WORLD);
            damageEventFilter = eventWorld.Filter<ApplyDamageEventComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in damageEventFilter)
            {
                var applyDamageEventComponent = applyDamagePool.Value.Get(eventEntity);
                if (applyDamageEventComponent.Target.Unpack(world,out int targetEntity))
                {
                    ref var unitComponent = ref unitPool.Value.Get(targetEntity);
                    unitComponent.Hp -= applyDamageEventComponent.Damage;
                    if (unitComponent.Hp<=0)
                    {
                        if (!deadPool.Value.Has(targetEntity))
                        {
                            deadPool.Value.Add(targetEntity);
                        }
                       
                    }
                }
            }
        }

       
    }
}