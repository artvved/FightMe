using System.Collections.Generic;
using DefaultNamespace.Game.Component.Spell;
using Game.Component;
using Game.Component.Time;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class ChainLightningAttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private EcsCustomInject<StaticData> staticData = default;
        private readonly EcsCustomInject<PositionService> service = default;


        private readonly EcsPoolInject<TickComponent> spellTickPool = default;
        private readonly EcsPoolInject<CreateAttackEventComponent> createAttackPool = Idents.Worlds.EVENT_WORLD;

        private EcsFilter spellEventFilter;
        private EcsFilter spellTickFilter;
        private EcsFilter enemyTransformFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);

            spellEventFilter = eventWorld.Filter<ChainLightningSpellEventComponent>().End();
            spellTickFilter = world.Filter<TickComponent>().Inc<ChainLightningSpellTag>().End();
            enemyTransformFilter = world.Filter<EnemyTag>().Inc<UnitViewComponent>().End();
            playerFilter = world.Filter<PlayerTag>().Inc<UnitViewComponent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ev in spellEventFilter)
            {
                foreach (var spellTick in spellTickFilter)
                {
                    ref var component = ref spellTickPool.Value.Get(spellTick);

                    if (component.CurrentTime >= component.FinalTime)
                    {
                        CastChainLightning();
                        component.CurrentTime = 0;
                    }
                }
            }
        }

        private void CastChainLightning()
        {
            var casterStats = staticData.Value.PlayerCasterStats;
            var targets = GetTargets(casterStats.ChainLightningTargetsRadius, casterStats.ChainLightningTargetsNumber);

            foreach (var target in targets)
            {
                ref var createAttackEventComponent = ref createAttackPool.NewEntity(out int eventEnt);
                        createAttackEventComponent.Target = world.PackEntity(target);
                        createAttackEventComponent.Damage = casterStats.ChainLightningDamage;
            }
        }

        private List<int> GetTargets(float radius, int targetCount)
        {
            List<int> randomTargets = new List<int>();
            foreach (var player in playerFilter)
            {
                List<int> targets = service.Value.GetTargetsInRange(player, enemyTransformFilter, radius);
                
                for (int i = 0; i < targetCount && targets.Count>0; i++)
                {
                    var random = Random.Range(0, targets.Count);
                    randomTargets.Add(targets[random]);
                    targets.RemoveAt(random);
                }
            }

            return randomTargets;
        }
    }
}