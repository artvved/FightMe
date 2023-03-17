using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class LifetimeSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        readonly EcsPoolInject<LifetimeComponent> lifetimePool = default;
        readonly EcsPoolInject<DeadTag> deadPool = default;

        private EcsFilter lifetimeFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            lifetimeFilter = world.Filter<LifetimeComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in lifetimeFilter)
            {
                ref var time = ref lifetimePool.Value.Get(entity);
                time.Value -= Time.deltaTime;
                if (time.Value<=0)
                {
                    deadPool.Value.Add(entity);
                }
            }
        }
    }
}