using DefaultNamespace.Game.Component.Spell;
using Game.Component;
using Game.Component.Time;
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

        private readonly EcsPoolInject<TickComponent> spellTickPool = default;
        private readonly EcsPoolInject<SpellButtonCDViewComponent> viewPool = default;
        

        private EcsFilter spellTickFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
           
            spellTickFilter = world.Filter<TickComponent>().Inc<SpellButtonCDViewComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in spellTickFilter)
            {
                var view = viewPool.Value.Get(entity).Value;
                
                var tickComponent = spellTickPool.Value.Get(entity);
                float p = Mathf.Clamp01((float) tickComponent.CurrentTime /(float) tickComponent.FinalTime);
                view.SetValue(p);
            }
        }
    }
}