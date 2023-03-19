using System.Collections.Generic;
using DefaultNamespace.Game.Component.Spell;
using Game.Component;
using Game.Component.Time;
using Game.Mono;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;

namespace Game.Service
{
    public class Fabric
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private StaticData staticData;
        private SceneData sceneData;


        private EcsPool<SpellButtonCDViewComponent> spellButtonViewPool;
        private EcsPool<UnitViewComponent> viewPool;
        private EcsPool<BaseViewComponent> baseViewPool;

        private EcsPool<PlayerTag> playerPool;
        private EcsPool<AllyTag> allyPool;
        private EcsPool<EnemyTag> enemyPool;
        private EcsPool<MeteorTag> meteorPool;
        private EcsPool<BossTag> bossPool;

        private EcsPool<ChainLightningSpellTag> chainLightningSpellPool;
        private EcsPool<MeteorSpellTag> meteorSpellPool;
        private EcsPool<MindControlSpellTag> mindControlSpellPool;
        
        private EcsPool<UnitComponent> unitPool;
        private EcsPool<SpeedComponent> speedPool;
        private EcsPool<MoveToPositionTargetComponent> moveToTargetPool;
        private EcsPool<DirectionComponent> dirPool;
        private EcsPool<CasterComponent> casterPool;

        private EcsPool<TickComponent> tickPool;

        private EcsPool<SpawnHpViewEventComponent> spawnHpViewEventPool;

        private EcsPool<HpRegenTickComponent> hpRegenPool;

        public Fabric(EcsWorld world, EcsWorld eventWorld, StaticData staticData, SceneData sceneData)
        {
            this.world = world;
            this.eventWorld = eventWorld;
            this.staticData = staticData;
            this.sceneData = sceneData;

            viewPool = world.GetPool<UnitViewComponent>();
            spellButtonViewPool = world.GetPool<SpellButtonCDViewComponent>();


            playerPool = world.GetPool<PlayerTag>();
            allyPool = world.GetPool<AllyTag>();
            enemyPool = world.GetPool<EnemyTag>();
            bossPool = world.GetPool<BossTag>();
            meteorPool = world.GetPool<MeteorTag>();

            chainLightningSpellPool = world.GetPool<ChainLightningSpellTag>();
            meteorSpellPool = world.GetPool<MeteorSpellTag>();
            mindControlSpellPool = world.GetPool<MindControlSpellTag>();

            unitPool = world.GetPool<UnitComponent>();
            speedPool = world.GetPool<SpeedComponent>();
            dirPool = world.GetPool<DirectionComponent>();
            moveToTargetPool = world.GetPool<MoveToPositionTargetComponent>();

            tickPool = world.GetPool<TickComponent>();

            casterPool = world.GetPool<CasterComponent>();
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
            unitComponent.Coins = stats.Coins;
            unitComponent.Range = stats.Range;
            unitComponent.AttackRatio = stats.AttackRatio;
            speedPool.Add(unitEntity).Value = stats.Speed;

            viewPool.Add(unitEntity).Value = view;
            baseViewPool.Add(unitEntity).Value = view;

            ref var hpRegenComponent = ref hpRegenPool.Add(unitEntity);
            hpRegenComponent.Value = stats.HpRegenValue;
            hpRegenComponent.Time = stats.HpRegenSpeed;

            spawnHpViewEventPool.Add(eventWorld.NewEntity()).Value = world.PackEntity(unitEntity);

            return unitEntity;
        }

        public int InstantiatePlayer()
        {
            var playerEntity = InstantiateUnit(staticData.PlayerPrefab, Vector3.zero, staticData.PlayerStats);
            playerPool.Add(playerEntity);
            allyPool.Add(playerEntity);
            ref var spells = ref casterPool.Add(playerEntity).SpellEntities;
            spells = new List<EcsPackedEntity>();


            var clSpell = AddSpell(chainLightningSpellPool, ref spells);
            spellButtonViewPool.Add(clSpell).Value = sceneData.ChainLightningButtonCdView;
            AddTickComponent(clSpell, staticData.PlayerCasterStats.ChainLightningCD);

            var mcSpell = AddSpell(mindControlSpellPool, ref spells);
            spellButtonViewPool.Add(mcSpell).Value = sceneData.MindControlButtonCdView;
            AddTickComponent(mcSpell, staticData.PlayerCasterStats.MindControlCD);

            var meteorSpell = AddSpell(meteorSpellPool, ref spells);
            spellButtonViewPool.Add(meteorSpell).Value = sceneData.MeteorButtonCdView;
            AddTickComponent(meteorSpell, staticData.PlayerCasterStats.MeteorCD);

            return playerEntity;
        }

        private int AddSpell<T>(EcsPool<T> pool, ref List<EcsPackedEntity> spells) where T : struct
        {
            var entity = world.NewEntity();
            pool.Add(entity);
            spells.Add(world.PackEntity(entity));
            return entity;
        }

        private void AddTickComponent(int entity, float finalTime)
        {
            ref var tickComponent = ref tickPool.Add(entity);
            tickComponent.CurrentTime = tickComponent.FinalTime = finalTime;
        }

        private void AddTickComponentFromStart(int entity, float finalTime)
        {
            ref var tickComponent = ref tickPool.Add(entity);
            tickComponent.CurrentTime = 0;
            tickComponent.FinalTime = finalTime;
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

        public int InstantiateMeteor(Vector3 targetPos)
        {
            Vector3 offset = new Vector3(0, 2, 0);
            var view = GameObject.Instantiate(staticData.MeteorPrefab);
            view.transform.position = targetPos + offset;

            int entity = world.NewEntity();
            view.Entity = entity;
            baseViewPool.Add(entity).Value = view;
            meteorPool.Add(entity);
            moveToTargetPool.Add(entity).Value=targetPos;
            dirPool.Add(entity).Value = Vector2.down;
            speedPool.Add(entity).Value = 5;

            return entity;
        }
    }
}