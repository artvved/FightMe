using Game.Component;
using Game.Component.View;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class GameOverDetectionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        private readonly EcsPoolInject<GameOverEventComponent> eventPool = Idents.Worlds.EVENT_WORLD;
        private EcsFilter deadPlayerFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            deadPlayerFilter = world.Filter<DeadTag>().Inc<PlayerTag>().End();
          
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var player in deadPlayerFilter)
            {
                eventPool.NewEntity(out int ev);
            }
        }

       
    }
}