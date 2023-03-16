using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class RegenTickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        private readonly EcsPoolInject<UnitComponent> unitPool = default;
        private readonly EcsPoolInject<HpRegenTickComponent> hpRegenPool = default;
        
        private EcsFilter attackTickFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            attackTickFilter = world.Filter<HpRegenTickComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in attackTickFilter)
            {
               ref var hpRegenTickComponent = ref hpRegenPool.Value.Get(entity);

               hpRegenTickComponent.CurrentTime += Time.deltaTime;

               if (hpRegenTickComponent.CurrentTime>=hpRegenTickComponent.Time)
               {
                   ref var unitComponent = ref unitPool.Value.Get(entity);
                   unitComponent.Hp += hpRegenTickComponent.Value;

                   if (unitComponent.Hp>unitComponent.MaxHp)
                   {
                       unitComponent.Hp = unitComponent.MaxHp;
                   }
                   
                   hpRegenTickComponent.CurrentTime = 0;
               }
            }
        }

       
    }
}