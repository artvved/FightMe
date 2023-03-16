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
    public class UpdateHpViewSystem : IEcsInitSystem, IEcsRunSystem, IEcsPostRunSystem
    {
        private EcsWorld world;

        private readonly EcsCustomInject<SceneData> sceneData = default;

        private readonly EcsPoolInject<HpViewComponent> hpViewPool = default;
        private readonly EcsPoolInject<UnitViewComponent> unitTransformPool = default;
        private readonly EcsPoolInject<UnitComponent> unitPool = default;

        private EcsFilter unitFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitFilter = world.Filter<HpViewComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitFilter)
            {
                ref HpView view = ref hpViewPool.Value.Get(entity).Value;

                var unitComponent = unitPool.Value.Get(entity);
                float p = (float) unitComponent.Hp /(float) unitComponent.MaxHp;
                view.SetValue(p);
            }
        }


        public void PostRun(IEcsSystems systems)
        {
            var camera = sceneData.Value.UICamera;
            foreach (var entity in unitFilter)
            {
                ref var view = ref hpViewPool.Value.Get(entity).Value;
                var pos = unitTransformPool.Value.Get(entity).Value.transform.position;

                var screenPoint = camera.WorldToScreenPoint(pos);
                view.transform.position = screenPoint;
            }
        }
    }
}