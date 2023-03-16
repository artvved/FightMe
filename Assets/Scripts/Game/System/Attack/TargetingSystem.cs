using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class TargetingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<AttackTargetComponent> attackTargetPool = default;
        private readonly EcsPoolInject<UnitViewComponent> unitTransformPool = default;
        private readonly EcsPoolInject<EnemyTag> enemyPool = default;
      

        private EcsFilter notAliveTargetAttackFilter;
        private EcsFilter targetAttackFilter;

        private EcsFilter allyTransformFilter;
        private EcsFilter enemyTransformFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            notAliveTargetAttackFilter = world.Filter<AttackTargetComponent>().End();
            targetAttackFilter = world.Filter<UnitComponent>().End();
            allyTransformFilter = world.Filter<AllyTag>().Inc<UnitViewComponent>().End();
            enemyTransformFilter = world.Filter<EnemyTag>().Inc<UnitViewComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            //clear not alive targets
            foreach (var entity in notAliveTargetAttackFilter)
            {
                if (!attackTargetPool.Value.Get(entity).Value.Unpack(world, out int alive))
                {
                    attackTargetPool.Value.Del(entity);
                }
            }

            foreach (var entity in targetAttackFilter)
            {
                var target = GetClosestTargetForEnemy(entity);
                if (target != -1)
                {
                    if (attackTargetPool.Value.Has(entity))
                    {
                        attackTargetPool.Value.Get(entity).Value = world.PackEntity(target);
                    }
                    else
                    {
                        attackTargetPool.Value.Add(entity).Value = world.PackEntity(target);
                    }
                    
                }
            }
        }

        private int GetClosestTargetForEnemy(int entity)
        {
            EcsFilter filter;
            //check if it is enemy
            if (enemyPool.Value.Has(entity))
            {
                filter = allyTransformFilter;
            }
            else
            {
                filter = enemyTransformFilter;
            }

            var entPos = unitTransformPool.Value.Get(entity).Value.transform.position;
            int closest = -1;
            float range = -1;
            foreach (var target in filter)
            {
                var allyPos = unitTransformPool.Value.Get(target).Value.transform.position;
                if (closest == -1 || (entPos - allyPos).magnitude < range)
                {
                    closest = target;
                    range = (entPos - allyPos).magnitude;
                }
            }

          
            return closest;
        }
    }
}