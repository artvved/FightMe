using System.Collections.Generic;
using DefaultNamespace;
using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class SpawnEnemySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsCustomInject<Fabric> fabric = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<EnemySpawnEventComponent> enemySpawnEventPool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<UnitViewComponent> playerTransformPool = default;
        private readonly EcsPoolInject<DirectionComponent> directionPool = default;
        private readonly EcsPoolInject<EnemySpawnedBeforeBossCountComponent> enemyCountPool = default;

        private EcsFilter eventFilter;
        private EcsFilter playerFilter;
        private EcsFilter enemyCountFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);

            playerFilter = world.Filter<PlayerTag>().Inc<UnitViewComponent>().End();
            eventFilter = eventWorld.Filter<EnemySpawnEventComponent>().End();
            enemyCountFilter = world.Filter<EnemySpawnedBeforeBossCountComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in eventFilter)
            {
                var enemySpawnEventComponent = enemySpawnEventPool.Value.Get(eventEntity);
                var count = enemySpawnEventComponent.Count;


                foreach (var playerEnt in playerFilter)
                {
                    var playerPos = playerTransformPool.Value.Get(playerEnt).Value.transform.position;
                    for (int i = 0; i < count; i++)
                    {
                        var range = Random.Range(0f, 2 * Mathf.PI);
                        var offset = new Vector3(3 * Mathf.Cos(range), 3 * Mathf.Sin(range));

                        foreach (var countEnt in enemyCountFilter)
                        {
                            ref var enemySpawnedCountComponent = ref enemyCountPool.Value.Get(countEnt);
                            if (enemySpawnedCountComponent.Value == staticData.Value.EnemiesBeforeBossCount )
                            {
                                fabric.Value.InstantiateBoss(playerPos + offset);
                                enemySpawnedCountComponent.Value=0;
                            }
                            else
                            {
                                fabric.Value.InstantiateEnemy(playerPos + offset);
                                enemySpawnedCountComponent.Value++;
                            }
                           
                        }
                    }
                }
            }
        }
    }
}