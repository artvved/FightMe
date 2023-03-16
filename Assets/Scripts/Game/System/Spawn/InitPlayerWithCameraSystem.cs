using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class InitPlayerWithCameraSystem : IEcsInitSystem
    {
        readonly EcsCustomInject<Fabric> fabric=default;
        

        public void Init(IEcsSystems systems)
        {
            var plEntity=fabric.Value.InstantiatePlayer();

        }


       
    }
}