using Game.Component;
using Game.Component.Time;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class TickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        private readonly EcsPoolInject<TickComponent> tickPool = default;
        private readonly EcsPoolInject<ChainLightningTickComponent> spellTickPool = default;
        
        private EcsFilter tickFilter;
        private EcsFilter spellTickFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            tickFilter = world.Filter<TickComponent>().End();
            spellTickFilter = world.Filter<ChainLightningTickComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in tickFilter)
            {
                ref var time = ref tickPool.Value.Get(entity);
                time.Value += Time.deltaTime;
            }
            foreach (var entity in spellTickFilter)
            {
                ref var time = ref spellTickPool.Value.Get(entity);
                time.CurrentTime += Time.deltaTime;
            }
        }
    }
}