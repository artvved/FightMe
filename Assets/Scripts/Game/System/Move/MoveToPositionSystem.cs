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
    public class MoveToPositionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        readonly EcsPoolInject<BaseViewComponent> transformPool = default;
        
        readonly EcsPoolInject<PositionTargetReachedTag> reachedPool = default;
        readonly EcsPoolInject<MoveToPositionTargetComponent> posTargetPool = default;

        private EcsFilter unitTransformFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            unitTransformFilter = world.Filter<MoveToPositionTargetComponent>()
                .Inc<SpeedComponent>()
                .Inc<BaseViewComponent>()
                .Exc<PositionTargetReachedTag>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in unitTransformFilter)
            {
                Vector2 position = transformPool.Value.Get(entity).Value.transform.position;
                var targetPosition = posTargetPool.Value.Get(entity).Value;

                
                if ((position.y-targetPosition.y)<0.001)
                {
                    reachedPool.Value.Add(entity);
                }
            }
        }
    }
}