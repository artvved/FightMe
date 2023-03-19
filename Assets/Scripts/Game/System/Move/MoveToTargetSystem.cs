using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class MoveToTargetSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        readonly EcsPoolInject<BaseViewComponent> transformPool = default;
        readonly EcsPoolInject<DirectionComponent> directionPool = default;
        readonly EcsPoolInject<AttackTargetComponent> targetPool = default;

        private EcsFilter unitTransformFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<AttackTargetComponent>()
                .Inc<SpeedComponent>()
                .Inc<BaseViewComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                targetPool.Value.Get(entity).Value.Unpack(world, out int target);
                var entPos = transformPool.Value.Get(entity).Value.transform.position;
                var targetPos = transformPool.Value.Get(target).Value.transform.position;
                Vector2 range = targetPos - entPos;
                
                var dir = range.normalized;

                if (directionPool.Value.Has(entity))
                {
                    directionPool.Value.Get(entity).Value = dir;
                }
                else
                {
                    directionPool.Value.Add(entity).Value = dir;
                }
            }
        }
    }
}