using Cinemachine;
using Game.Service;
using Game.UI;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;

namespace ScriptableData
{
    public class SceneData : MonoBehaviour
    {
        public EcsUguiEmitter EcsUguiEmitter;
        public Canvas HpCanvas;
        public Camera UICamera;
        
        public CoinsView CoinsView;
        public SpellButtonCDView ChainLightningButtonCdView;
        public SpellButtonCDView MindControlButtonCdView;
        public SpellButtonCDView MeteorButtonCdView;

        public GameOverPopupView GameOverPopupView;


    }
}