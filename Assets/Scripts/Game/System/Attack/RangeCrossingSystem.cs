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
        private EcsWorld eventWorld;
      
        readonly EcsPoolInject<BaseViewComponent> transformPool=default;
        readonly EcsPoolInject<UnitComponent> unitPool=default;
        readonly EcsPoolInject<CantMoveComponent> cantMovePool = default;
        readonly EcsPoolInject<AttackTargetComponent> attackingPool = default;
        readonly EcsPoolInject<AttackTickComponent> attackingTickPool = default;
        
        readonly EcsPoolInject<DeadTag> deadPool = default;
        readonly EcsPoolInject<ApplyDamageEventComponent> applyDamageEventPool = Startup.EVENT_WORLD;
      

        private EcsFilter notAttackingEnemyTransformFilter;
        private EcsFilter enemyTransformFilter;
        private EcsFilter playerFilter;
        private EcsFilter projectileFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Startup.EVENT_WORLD);
            
            playerFilter = world.Filter<PlayerTag>().End();
            notAttackingEnemyTransformFilter = world.Filter<UnitViewComponent>()
                .Inc<UnitComponent>()
                .Exc<PlayerTag>()
                .Exc<AttackTargetComponent>()
                .End();
            
            enemyTransformFilter = world.Filter<UnitViewComponent>()
                .Inc<UnitComponent>()
                .Exc<PlayerTag>()
                .End();

         
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var playerEnt in playerFilter)
            {
                var playerPos = transformPool.Value.Get(playerEnt).Value.transform.position;
                var playerRange= unitPool.Value.Get(playerEnt).Range;

                foreach (var entity in notAttackingEnemyTransformFilter)
                {
                    var enemyRange = unitPool.Value.Get(entity).Range;
                    var enemyPos = transformPool.Value.Get(entity).Value.transform.position;

                    if (IsInRange(playerPos,enemyPos,enemyRange))
                    {
                        cantMovePool.Value.Add(entity);
                        attackingPool.Value.Add(entity).Value = world.PackEntity(playerEnt);
                        attackingTickPool.Value.Add(entity);
                    }
                }
                
                ManagePlayerAttack(playerEnt,playerPos,playerRange);
            }
            
            
        }
        
        private void ManagePlayerAttack(int playerEnt,Vector3 playerPos,float playerRange){

            if (attackingPool.Value.Has(playerEnt) && !attackingPool.Value.Get(playerEnt).Value.Unpack(world,out int target))
            {
                attackingPool.Value.Del(playerEnt);
                attackingTickPool.Value.Del(playerEnt);
            }
            
            int closestEnemy = -1;
            float closestEnemyDistance=999f;

            foreach (var entity in enemyTransformFilter)
            {
                var enemyPos = transformPool.Value.Get(entity).Value.transform.position;

                //find nearest enemy
                if (Mathf.Abs(playerPos.x - enemyPos.x) < closestEnemyDistance)
                {
                    closestEnemy = entity;
                    closestEnemyDistance = Mathf.Abs(playerPos.x - enemyPos.x);
                }
            }
            
            //enemy exists and in player range and not attacking already
            if (closestEnemy!= -1 && closestEnemyDistance<=playerRange && !attackingPool.Value.Has(playerEnt))
            {
                attackingPool.Value.Add(playerEnt).Value = world.PackEntity(closestEnemy);
                attackingTickPool.Value.Add(playerEnt);
            }
        }

        private bool IsInRange(Vector3 pos1, Vector3 pos2, float range)
        {
            return Mathf.Abs(pos1.x - pos2.x) <= range;
        }
    }
}