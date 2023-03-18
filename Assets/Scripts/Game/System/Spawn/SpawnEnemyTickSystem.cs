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

        private readonly EcsPoolInject<EnemySpawnEventComponent> enemySpawnEventPool = Idents.Worlds.EVENT_WORLD;

        private readonly EcsPoolInject<TickComponent> tickPool = default;
        private readonly EcsPoolInject<EnemySpawnedBeforeBossCountComponent> enemyCountPool = default;

        private EcsFilter filter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            
            filter = world.Filter<TickComponent>().Inc<EnemySpawnedBeforeBossCountComponent>().End();

            var newEntity = world.NewEntity();
            tickPool.Value.Add(newEntity).FinalTime=staticData.Value.EnemySpawnPeriod;
            enemyCountPool.Value.Add(newEntity);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                ref var tickComponent = ref tickPool.Value.Get(entity);
                if (tickComponent.CurrentTime >= tickComponent.FinalTime)
                {
                    tickComponent.CurrentTime = 0;
                    enemySpawnEventPool.NewEntity(out int e).Count=1;
                }
            }
        }
    }
}