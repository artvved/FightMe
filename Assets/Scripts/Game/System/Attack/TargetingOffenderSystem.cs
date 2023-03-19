using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class TargetingOffenderSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
      
        
        private readonly EcsPoolInject<CreateAttackEventComponent> attackEventPool = Idents.Worlds.EVENT_WORLD;

        private readonly EcsPoolInject<AttackTargetComponent> attackTargetPool = default;
        private readonly EcsPoolInject<AllyTag> allyPool = default;

        private EcsFilter createAttackEventFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);
            
            createAttackEventFilter = eventWorld.Filter<CreateAttackEventComponent>().End();
            
        }
        

        public void Run(IEcsSystems systems)
        {
            foreach (var atkEvent in createAttackEventFilter)
            {
                var attackEventComponent = attackEventPool.Value.Get(atkEvent);

                if (attackEventComponent.Sender.Unpack(world,out int sender) 
                    && allyPool.Value.Has(sender)
                    && attackEventComponent.Target.Unpack(world,out int target))
                {
                    attackTargetPool.Value.Get(target).Value = attackEventComponent.Sender;
                }
                
            }
            
        }
    }
}