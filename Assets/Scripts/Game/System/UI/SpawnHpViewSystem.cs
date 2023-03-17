using DefaultNamespace;
using Game.Component;
using Game.Component.View;
using Game.Service;
using Game.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class SpawnHpViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<SpawnHpViewEventComponent> spawnEventPool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<HpViewComponent> hpViewPool = default;
        private readonly EcsPoolInject<UnitComponent> unitPool = default;

        private EcsFilter spawnEventFilter;
      


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);

            spawnEventFilter = eventWorld.Filter<SpawnHpViewEventComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in spawnEventFilter)
            {
                if (!spawnEventPool.Value.Get(eventEntity).Value.Unpack(world,out int entity))
                {
                    return;
                }

                HpView view= GameObject.Instantiate(staticData.Value.HpViewPrefab, sceneData.Value.HpCanvas.transform);
                hpViewPool.Value.Add(entity).Value = view;
                
                var unitComponent = unitPool.Value.Get(entity);
                var p = unitComponent.Hp / unitComponent.MaxHp;
                view.SetValue(p);
            }
            
        }
        
    }
}