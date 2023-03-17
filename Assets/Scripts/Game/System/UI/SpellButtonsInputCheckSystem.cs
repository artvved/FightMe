using Game.Component;
using Game.Component.View;
using Game.Service;
using Game.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;
using UnityEngine.Scripting;


namespace Game.System
{
    public class SpellButtonsInputCheckSystem : EcsUguiCallbackSystem , IEcsInitSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private readonly EcsPoolInject<ChainLightningSpellEventComponent> eventPool = Idents.Worlds.EVENT_WORLD;


     
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.Worlds.EVENT_WORLD);
        }

        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.ChainLightning, Idents.Worlds.EVENT_WORLD)]
        void OnClickChainLightning (in EcsUguiClickEvent e)
        {
            eventPool.NewEntity(out var entity);
        }
    }
}