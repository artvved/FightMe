using DefaultNamespace;
using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class LifestealSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private readonly EcsCustomInject<StaticData> data = default;


        private readonly EcsPoolInject<ApplyDamageEventComponent> applyDamagePool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<CreateAttackEventComponent> createAttackPool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<CasterComponent> casterPool = default;
        private readonly EcsPoolInject<UnitComponent> unitPool = default;
        
        private EcsFilter eventFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);
            eventFilter = eventWorld.Filter<CreateAttackEventComponent>().End();
           
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                var attackEventComponent = createAttackPool.Value.Get(entity);

                if (attackEventComponent.Sender.Unpack(world,out int caster) 
                    && casterPool.Value.Has(caster)
                    && attackEventComponent.Target.Unpack(world,out int target))
                {
                    ref var casterHp = ref unitPool.Value.Get(caster).Hp;
                    var maxHp = unitPool.Value.Get(caster).MaxHp;
                    var targetMaxHp = unitPool.Value.Get(target).MaxHp;
                    var lifestealMult = data.Value.PlayerCasterStats.LifestealPercent / 100;

                    casterHp += (int)(targetMaxHp * lifestealMult);
                    if (casterHp>maxHp)
                    {
                        casterHp = maxHp;
                    }
                   
                }
               
            }
        }
    }
}