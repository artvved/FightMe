using Game.Component;
using Game.Component.View;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Game.System
{
    public class CoinGainSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        
        private readonly EcsPoolInject<UnitComponent> unitPool = default;
        private readonly EcsPoolInject<CoinsChangedEventComponent> eventPool = Idents.Worlds.EVENT_WORLD;
        
        private EcsFilter deadFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            deadFilter = world.Filter<DeadTag>().Inc<UnitComponent>().Exc<PlayerTag>().End();
            playerFilter = world.Filter<PlayerTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var player in playerFilter)
            {
                ref var coins = ref unitPool.Value.Get(player).Coins;
                int i = 0;
                foreach (var unit in deadFilter)
                {
                    coins += unitPool.Value.Get(unit).Coins;
                    i++;
                }
                
                if (i>0)
                {
                    eventPool.NewEntity(out int entity);
                }
              
            }
        }

       
    }
}