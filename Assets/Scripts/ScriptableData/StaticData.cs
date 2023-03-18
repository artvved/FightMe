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
        public UnitStats BossStats;
        public CasterStats PlayerCasterStats;
        
        public PlayerView PlayerPrefab;
        public EnemyView EnemyPrefab;
        public EnemyView BossPrefab;
        public BaseView MeteorPrefab;
        
        public float EnemySpawnPeriod;
        public int EnemiesBeforeBossCount;
      
        //ui
        [Header("UI")]
        public HpView HpViewPrefab;
        public DamageView DamageViewPrefab;
    }
}