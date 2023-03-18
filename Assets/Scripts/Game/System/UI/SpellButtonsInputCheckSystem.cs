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
    public class SpellButtonsInputCheckSystem : EcsUguiCallbackSystem 
    {
        private readonly EcsPoolInject<ChainLightningSpellEventComponent> clEventPool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<MindControlSpellEventComponent> mindControlEventPool = Idents.Worlds.EVENT_WORLD;
        private readonly EcsPoolInject<MeteorSpellEventComponent> meteorEventPool = Idents.Worlds.EVENT_WORLD;
        

        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.ChainLightning, Idents.Worlds.EVENT_WORLD)]
        void OnClickChainLightning (in EcsUguiClickEvent e)
        {
            clEventPool.NewEntity(out var entity);
        }
        
        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.MindControl, Idents.Worlds.EVENT_WORLD)]
        void OnClickMindControl (in EcsUguiClickEvent e)
        {
            mindControlEventPool.NewEntity(out var entity);
        }
        
        [Preserve]
        [EcsUguiClickEvent (Idents.Ui.Meteor, Idents.Worlds.EVENT_WORLD)]
        void OnClickMeteor (in EcsUguiClickEvent e)
        {
            meteorEventPool.NewEntity(out var entity);
        }
    }
}