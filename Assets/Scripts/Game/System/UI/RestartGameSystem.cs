using Game.Component;
using Game.Component.Time;
using Game.Component.View;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Game.System
{
    public class RestartGameSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<TickComponent> tickPool = default;
      
        private EcsFilter restartTickFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();

            restartTickFilter = world.Filter<TickComponent>().Inc<GameOverViewComponent>().End();
          
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ent in restartTickFilter)
            {
                var tickComponent = tickPool.Value.Get(ent);
                if (tickComponent.CurrentTime>=tickComponent.FinalTime)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }

       
    }
}