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
    public class MindControlAttackSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private EcsCustomInject<StaticData> staticData = default;
        private readonly EcsCustomInject<PositionService> service = default;

        private readonly EcsPoolInject<TickComponent> tickPool = default;
        private readonly EcsPoolInject<EnemyTag> enemyPool = default;
        private readonly EcsPoolInject<AllyTag> allyPool = default;
        private readonly EcsPoolInject<MindControlEffectComponent> effectPool = default;

        private EcsFilter spellEventFilter;
        private EcsFilter spellTickFilter;
        private EcsFilter effectDurationTickFilter;
        private EcsFilter enemyNotBossFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);

            spellEventFilter = eventWorld.Filter<MindControlSpellEventComponent>().End();
            effectDurationTickFilter = world.Filter<TickComponent>().Inc<MindControlEffectComponent>().End();
            spellTickFilter = world.Filter<TickComponent>().Inc<MindControlSpellTag>().End();
            enemyNotBossFilter = world.Filter<EnemyTag>().Inc<UnitViewComponent>().Exc<BossTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ev in spellEventFilter)
            {
                foreach (var spellTick in spellTickFilter)
                {
                    ref var component = ref tickPool.Value.Get(spellTick);

                    if (component.CurrentTime >= component.FinalTime)
                    {
                        CastMindControl();
                        component.CurrentTime = 0;
                    }
                }
            }

            foreach (var effect in effectDurationTickFilter)
            {
                ref var component = ref tickPool.Value.Get(effect);

                if (component.CurrentTime >= component.FinalTime)
                {
                    UnCastMindControl(effect);
                }
            }
        }

        private void CastMindControl()
        {
            var casterStats = staticData.Value.PlayerCasterStats;
            var randomTarget = service.Value.GetRandomTarget(enemyNotBossFilter);

            enemyPool.Value.Del(randomTarget);
            allyPool.Value.Add(randomTarget);

            var effect = world.NewEntity();
            effectPool.Value.Add(effect).Target = world.PackEntity(randomTarget);
            tickPool.Value.Add(effect).FinalTime = casterStats.MindControlDuration;
        }

        private void UnCastMindControl(int effect)
        {
            if (effectPool.Value.Get(effect).Target.Unpack(world, out int target))
            {
                enemyPool.Value.Add(target);
                allyPool.Value.Del(target);
            }

            world.DelEntity(effect);
        }
    }
}