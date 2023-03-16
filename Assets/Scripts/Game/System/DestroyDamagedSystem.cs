using Game.Component;
using Game.Component.View;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class DestroyDamagedSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<BaseViewComponent> viewPool = default;
        private readonly EcsPoolInject<HpViewComponent> hpViewPool = default;
        private readonly EcsPoolInject<DamageViewComponent> damageViewPool = default;
        private EcsFilter deadFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            deadFilter = world.Filter<DeadTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in deadFilter)
            {
                if (viewPool.Value.Has(unit))
                {
                    GameObject.Destroy(viewPool.Value.Get(unit).Value.gameObject);
                }
                if (hpViewPool.Value.Has(unit))
                {
                    GameObject.Destroy(hpViewPool.Value.Get(unit).Value.gameObject);
                }
                if (damageViewPool.Value.Has(unit))
                {
                    GameObject.Destroy(damageViewPool.Value.Get(unit).Value.gameObject);
                }
                world.DelEntity(unit);
               
            }
        }

       
    }
}