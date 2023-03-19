using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class TargetingClosestSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsCustomInject<PositionService> service = default;
        private readonly EcsPoolInject<AttackTargetComponent> attackTargetPool = default;
        private readonly EcsPoolInject<AttackTickComponent> attackTickPool = default;
        private readonly EcsPoolInject<EnemyTag> enemyPool = default;
        private readonly EcsPoolInject<AllyTag> allyPool = default;

        private EcsFilter nonValidTargetAttackFilter;
        private EcsFilter targetAttackFilter;

        private EcsFilter allyTransformFilter;
        private EcsFilter enemyTransformFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            nonValidTargetAttackFilter = world.Filter<AttackTargetComponent>().End();
            targetAttackFilter = world.Filter<UnitComponent>().End();
            allyTransformFilter = world.Filter<AllyTag>().Inc<UnitViewComponent>().End();
            enemyTransformFilter = world.Filter<EnemyTag>().Inc<UnitViewComponent>().End();
        }

        private bool CheckSides(int ent, int target)
        {
            return (enemyPool.Value.Has(ent) && allyPool.Value.Has(target))
                   || (allyPool.Value.Has(ent) && enemyPool.Value.Has(target));
        }

        public void Run(IEcsSystems systems)
        {
            //clear not alive and wrong targets
            foreach (var entity in nonValidTargetAttackFilter)
            {
                if (!attackTargetPool.Value.Get(entity).Value.Unpack(world, out int target))
                {
                    attackTargetPool.Value.Del(entity);
                    attackTickPool.Value.Del(entity);
                }
                else if (!CheckSides(entity, target))
                {
                    attackTargetPool.Value.Del(entity);
                    attackTickPool.Value.Del(entity);
                }
            }

            foreach (var entity in targetAttackFilter)
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

                var target = service.Value.GetClosestTarget(entity, filter);

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
    }
}