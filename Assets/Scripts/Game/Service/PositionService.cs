using System.Collections.Generic;
using Game.Component;
using Game.Mono;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Game.Service
{
    public class PositionService
    {
        private EcsWorld world;

        private EcsPool<BaseViewComponent> baseTransformPool;
        private EcsPool<PlayerTag> playerPool;
        private EcsPool<AllyTag> allyPool;
        private EcsPool<EnemyTag> enemyPool;
        private EcsPool<BossTag> bossPool;

        public PositionService(EcsWorld world)
        {
            this.world = world;

            baseTransformPool = world.GetPool<BaseViewComponent>();
            
            playerPool = world.GetPool<PlayerTag>();
            allyPool = world.GetPool<AllyTag>();
            enemyPool = world.GetPool<EnemyTag>();
            bossPool = world.GetPool<BossTag>();
            
        }
        
        public int GetClosestTarget(int entity,EcsFilter filter)
        {
            var entPos = baseTransformPool.Get(entity).Value.transform.position;
            int closest = -1;
            float range = -1;
            foreach (var target in filter)
            {
                var allyPos = baseTransformPool.Get(target).Value.transform.position;
                if (closest == -1 || (entPos - allyPos).magnitude < range)
                {
                    closest = target;
                    range = (entPos - allyPos).magnitude;
                }
            }
            
            return closest;
        }
        
        public int GetRandomTarget(EcsFilter filter)
        {
            List<int> list = new List<int>();
            foreach (var target in filter)
            {
                list.Add(target);
            }
            
            return list[Random.Range(0,list.Count)];
        }
        
        public List<int> GetTargetsInRange(int entity,EcsFilter filter,float range)
        {
            List<int> list = new List<int>();
            var entPos = baseTransformPool.Get(entity).Value.transform.position;
            
            foreach (var target in filter)
            {
                var pos2 = baseTransformPool.Get(target).Value.transform.position;
               
                if (IsInRange(entPos,pos2,range))
                {
                    list.Add(target);
                }
            }
            
            return list;
        }

        public int GetClosestTargetWithRange(int entity, EcsFilter filter,float range)
        {
            var closestTarget = GetClosestTarget(entity, filter);
            var pos1 = baseTransformPool.Get(entity).Value.transform.position;
            var pos2 = baseTransformPool.Get(closestTarget).Value.transform.position;
            
            if (!IsInRange(pos1,pos2,range))
            {
                return -1;
            }

            return closestTarget;
        }

        public bool IsInRange(Vector3 pos1, Vector3 pos2, float range)
        {
            return (pos1 - pos2).magnitude <= range;
        }


       
    }
}