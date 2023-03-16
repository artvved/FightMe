using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Game.System
{
    public class RangeCrossingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        readonly EcsPoolInject<BaseViewComponent> transformPool=default;
        readonly EcsPoolInject<UnitComponent> unitPool=default;
        readonly EcsPoolInject<CantMoveComponent> cantMovePool = default;
        readonly EcsPoolInject<AttackTargetComponent> targetPool = default;
        readonly EcsPoolInject<AttackTickComponent> attackingTickPool = default;

      

        private EcsFilter movingUnitFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            movingUnitFilter = world.Filter<UnitViewComponent>()
                .Inc<AttackTargetComponent>()
                .Inc<UnitComponent>()
                .Exc<AttackTickComponent>()
                .End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in movingUnitFilter)
            {
                var range = unitPool.Value.Get(unit).Range;
                targetPool.Value.Get(unit).Value.Unpack(world,out int target);
                var position = transformPool.Value.Get(unit).Value.transform.position;
                var targetPosition = transformPool.Value.Get(target).Value.transform.position;

                var inRange = IsInRange(position, targetPosition, range);
                if (inRange)
                {
                    cantMovePool.Value.Add(unit);
                    attackingTickPool.Value.Add(unit);
                }
                
                if (!inRange && cantMovePool.Value.Has(unit))
                {
                    cantMovePool.Value.Del(unit);
                }
            }
            
            
        }
        
       

        private bool IsInRange(Vector3 pos1, Vector3 pos2, float range)
        {
            return (pos1 - pos2).magnitude <= range;
    }
    }
}