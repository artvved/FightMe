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
    public class Fabric
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private StaticData staticData;


        private EcsPool<UnitViewComponent> viewPool;
        private EcsPool<PlayerTag> playerPool;
        private EcsPool<AllyTag> allyPool;
        private EcsPool<EnemyTag> enemyPool;
        private EcsPool<BossTag> bossPool;
        
        private EcsPool<UnitComponent> unitPool;
        private EcsPool<SpeedComponent> speedPool;
        private EcsPool<ChainLightningTickComponent> chainLightningTickPool;
       
        private EcsPool<LifetimeComponent> lifetimePool;
        private EcsPool<BaseViewComponent> baseViewPool;
        private EcsPool<SpawnHpViewEventComponent> spawnHpViewEventPool;
       
        private EcsPool<HpRegenTickComponent> hpRegenPool;
            
        public Fabric(EcsWorld world,EcsWorld eventWorld,  StaticData staticData)
        {
            this.world = world;
            this.eventWorld = eventWorld;
            this.staticData = staticData;

            viewPool = world.GetPool<UnitViewComponent>();
            
            playerPool = world.GetPool<PlayerTag>();
            allyPool = world.GetPool<AllyTag>();
            enemyPool = world.GetPool<EnemyTag>();
            bossPool = world.GetPool<BossTag>();
            
            unitPool = world.GetPool<UnitComponent>();
            speedPool = world.GetPool<SpeedComponent>();
            chainLightningTickPool = world.GetPool<ChainLightningTickComponent>();
           
            lifetimePool = world.GetPool<LifetimeComponent>();
            baseViewPool = world.GetPool<BaseViewComponent>();
           
            hpRegenPool = world.GetPool<HpRegenTickComponent>();
            
            spawnHpViewEventPool = eventWorld.GetPool<SpawnHpViewEventComponent>();
        }


        private int InstantiateUnit(UnitView prefab, Vector3 position, UnitStats stats)
        {
            var view = GameObject.Instantiate(prefab);
            view.transform.position = position;
            int unitEntity = world.NewEntity();
            view.Entity = unitEntity;

            ref var unitComponent = ref unitPool.Add(unitEntity);
            unitComponent.MaxHp = unitComponent.Hp = stats.MaxHp;
            unitComponent.Damage = stats.Damage;
            unitComponent.Coins = stats.Coins ;
            unitComponent.Range = stats.Range;
            unitComponent.AttackRatio = stats.AttackRatio;
            speedPool.Add(unitEntity).Value = stats.Speed;

            viewPool.Add(unitEntity).Value = view;
            baseViewPool.Add(unitEntity).Value = view;
            
            ref var hpRegenComponent = ref hpRegenPool.Add(unitEntity);
            hpRegenComponent.Value = stats.HpRegenValue;
            hpRegenComponent.Time = stats.HpRegenSpeed;

            spawnHpViewEventPool.Add(eventWorld.NewEntity()).Value=world.PackEntity(unitEntity);

            return unitEntity;
        }

        public int InstantiatePlayer()
        {
            var playerEntity = InstantiateUnit(staticData.PlayerPrefab, Vector3.zero, staticData.PlayerStats);
            playerPool.Add(playerEntity);
            allyPool.Add(playerEntity);
            ref var chainLightningTickComponent = ref chainLightningTickPool.Add(playerEntity);
            chainLightningTickComponent.CurrentTime =
                chainLightningTickComponent.Time = staticData.PlayerCasterStats.ChainLightningCD;
            return playerEntity;
        }

        public int InstantiateEnemy(Vector3 pos)
        {
            var enemyEntity = InstantiateUnit(staticData.EnemyPrefab, pos, staticData.EnemyStats);
            enemyPool.Add(enemyEntity);
            return enemyEntity;
        }


        public int InstantiateBoss(Vector3 pos)
        {
            var enemyEntity = InstantiateUnit(staticData.BossPrefab, pos, staticData.BossStats);
            enemyPool.Add(enemyEntity);
            bossPool.Add(enemyEntity);
            return enemyEntity;
        }
    }
}