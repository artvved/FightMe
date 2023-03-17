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
    public class UpdateSpellButtonsCDSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private readonly EcsCustomInject<SceneData> sceneData = default;

        private readonly EcsPoolInject<ChainLightningTickComponent> spellTickPool = default;
        

        private EcsFilter spellTickFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
           
            spellTickFilter = world.Filter<ChainLightningTickComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in spellTickFilter)
            {
                var view = sceneData.Value.ChainLightningButtonCdView;
                
                var tickComponent = spellTickPool.Value.Get(entity);
                float p = (float) tickComponent.CurrentTime /(float) tickComponent.Time;
                view.SetValue(p);
            }
        }
    }
}