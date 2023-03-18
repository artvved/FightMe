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
    public class MeteorAttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private EcsCustomInject<Fabric> fabric = default;
        private EcsCustomInject<PositionService> posService = default;
        private EcsCustomInject<StaticData> staticData = default;


        private readonly EcsPoolInject<CreateAttackEventComponent> createAttackPool = Idents.Worlds.EVENT_WORLD;

        private readonly EcsPoolInject<TickComponent> spellTickPool = default;
        private readonly EcsPoolInject<DeadTag> deadPool = default;

        private EcsFilter spellEventFilter;
        private EcsFilter spellTickFilter;
        private EcsFilter meteorHitFilter;
        private EcsFilter enemyFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);

            spellEventFilter = eventWorld.Filter<MeteorSpellEventComponent>().End();
            spellTickFilter = world.Filter<TickComponent>().Inc<MeteorSpellTag>().End();
            meteorHitFilter = world.Filter<MeteorTag>().Inc<PositionTargetReachedTag>().End();
            enemyFilter = world.Filter<EnemyTag>().Inc<UnitViewComponent>().End();
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
                        CastMeteor();
                        component.CurrentTime = 0;
                    }
                }
            }

            var stats = staticData.Value.PlayerCasterStats;
            foreach (var meteor in meteorHitFilter)
            {
              
                var targetsInRange = posService.Value.GetTargetsInRange(meteor, enemyFilter, stats.MeteorTargetsRadius);

                foreach (var target in targetsInRange)
                {
                    ref var createAttackEventComponent = ref createAttackPool.NewEntity(out int eventEnt);
                    createAttackEventComponent.Target = world.PackEntity(target);
                    createAttackEventComponent.Damage = (int) (stats.MeteorDamage + stats.MeteorAdditionalDamageMultiplier *
                        targetsInRange.Count);
                }

                deadPool.Value.Add(meteor);
            }
        }

        private void CastMeteor()
        {
            fabric.Value.InstantiateMeteor(new Vector2(Random.Range(-2f, 2f), Random.Range(-4f, 4f)));
        }
    }
}