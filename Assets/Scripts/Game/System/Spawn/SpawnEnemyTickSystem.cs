using System.Collections.Generic;
using Game.Component;
using Game.Component.Time;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class SpawnEnemyTickSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<EnemySpawnEventComponent> enemySpawnEventPool = Startup.EVENT_WORLD;

        private readonly EcsPoolInject<TickComponent> tickPool = default;
        private readonly EcsPoolInject<SpawnEnemyTickComponent> spawnEnemyTickPool = default;
        private readonly EcsPoolInject<EnemySpawnedBeforeBossCountComponent> enemyCountPool = default;

        private EcsFilter filter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();


            filter = world.Filter<TickComponent>().Inc<SpawnEnemyTickComponent>().End();

            var newEntity = world.NewEntity();
            tickPool.Value.Add(newEntity);
            spawnEnemyTickPool.Value.Add(newEntity).Value = staticData.Value.EnemySpawnPeriod;
            enemyCountPool.Value.Add(newEntity);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                ref var tickComponent = ref tickPool.Value.Get(entity);
                var spawnTickComponent = spawnEnemyTickPool.Value.Get(entity);
                if (tickComponent.Value >= spawnTickComponent.Value)
                {
                    tickComponent.Value = 0;
                    enemySpawnEventPool.NewEntity(out int e).Count=1;
                }
            }
        }
    }
}