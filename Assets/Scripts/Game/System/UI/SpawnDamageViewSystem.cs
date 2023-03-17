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
    public class SpawnDamageViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<ApplyDamageEventComponent> eventPool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<UnitViewComponent> unitTransformPool = default;
        private readonly EcsPoolInject<LifetimeComponent> lifetimePool = default;
        private readonly EcsPoolInject<DamageViewComponent> damageViewPool = default;

        private EcsFilter eventFilter;
      


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);

            eventFilter = eventWorld.Filter<ApplyDamageEventComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            var camera = sceneData.Value.UICamera;
            foreach (var eventEntity in eventFilter)
            {
                var applyDamageEventComponent = eventPool.Value.Get(eventEntity);
                if (!applyDamageEventComponent.Target.Unpack(world,out int target))
                {
                    return;
                }
                
                var view= GameObject.Instantiate(staticData.Value.DamageViewPrefab, sceneData.Value.HpCanvas.transform);
                var newEntity = world.NewEntity();
                
                lifetimePool.Value.Add(newEntity).Value = view.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;;
                damageViewPool.Value.Add(newEntity).Value = view;
                
                var pos = unitTransformPool.Value.Get(target).Value.transform.position;

                var screenPoint = camera.WorldToScreenPoint(pos);
                view.transform.position = screenPoint;
                view.TextMeshProUGUI.text = applyDamageEventComponent.Damage.ToString();

            }
            
        }
        
    }
}