using Game.Mono;
using Game.UI;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        public UnitStats PlayerStats;
        public UnitStats EnemyStats;
        
        public PlayerView PlayerPrefab;
        public EnemyView EnemyPrefab;
        
        public float EnemySpawnPeriod;
      
        //ui
        [Header("UI")]
        public HpView HpViewPrefab;
        public DamageView DamageViewPrefab;
    }
}