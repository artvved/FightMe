using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class AttackTickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        private readonly EcsPoolInject<AttackTickComponent> tickPool = default;
        private readonly EcsPoolInject<AttackTargetComponent> attackTargetPool = default;
        private readonly EcsPoolInject<UnitComponent> unitPool = default;
        //private readonly EcsPoolInject<ApplyDamageEventComponent> applyDamagePool = Startup.EVENT_WORLD;
        private readonly EcsPoolInject<CreateAttackEventComponent> createAttackPool = Startup.EVENT_WORLD;

        private EcsFilter attackTickFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            attackTickFilter = world.Filter<AttackTickComponent>().Inc<AttackTargetComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in attackTickFilter)
            {
                ref var attackTickComponent = ref tickPool.Value.Get(entity);
                

                var ratio = unitPool.Value.Get(entity).AttackRatio;

                if (attackTickComponent.Value <= 0)
                {
                    ref var createAttackEventComponent = ref createAttackPool.NewEntity(out int eventEnt);
                    createAttackEventComponent.Creator = world.PackEntity(entity);
                    createAttackEventComponent.Target = attackTargetPool.Value.Get(entity).Value;
                    createAttackEventComponent.Damage = unitPool.Value.Get(entity).Damage;
                        
                    attackTickComponent.Value = 1f/ratio;
                 
                }
                attackTickComponent.Value -= Time.deltaTime;
            }
        }

       
    }
}