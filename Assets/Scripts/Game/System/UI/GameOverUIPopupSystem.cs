using Game.Component;
using Game.Component.Time;
using Game.Component.View;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class GameOverUIPopupSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsCustomInject<SceneData> sceneData = default;

        private readonly EcsPoolInject<TickComponent> tickPool = default;
        private readonly EcsPoolInject<GameOverViewComponent> viewPool = default;

        private EcsFilter gameOverEventFilter;
       

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);
            
            gameOverEventFilter = eventWorld.Filter<GameOverEventComponent>().End();
          
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ev in gameOverEventFilter)
            {
                var animator = sceneData.Value.GameOverPopupView.Animator;
                animator.SetTrigger("Popup");
                var newEntity = world.NewEntity();
                tickPool.Value.Add(newEntity).FinalTime = animator.GetCurrentAnimatorStateInfo(0).length+2;
                viewPool.Value.Add(newEntity).Value = sceneData.Value.GameOverPopupView;
            }
        }

       
    }
}