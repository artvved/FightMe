using DefaultNamespace;
using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class CreateDamageSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

       
        private readonly EcsPoolInject<ApplyDamageEventComponent> applyDamagePool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<CreateAttackEventComponent> createAttackPool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<PlayerTag> playerPool = default;
      
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
               //Debug.Log(entity + " e  "+" tar"+createAttackPool.Value.Get(entity).Target+"  d mg"+ createAttackPool.Value.Get(entity).Damage);
                
                ref var applyDamageEventComponent = ref applyDamagePool.NewEntity(out int eventEnt);
                applyDamageEventComponent.Target = createAttackPool.Value.Get(entity).Target;
                applyDamageEventComponent.Damage = createAttackPool.Value.Get(entity).Damage;
            }
        }
    }
}