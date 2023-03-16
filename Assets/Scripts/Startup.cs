using System;
using System.Collections;
using System.Collections.Generic;
using Game.Component;
using Game.Service;
using Game.System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;

public class Startup : MonoBehaviour
{
    private EcsWorld world;
    private EcsSystems systems;
    public const string EVENT_WORLD = "Event";

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private StaticData staticData;
    void Start()
    {
        world = new EcsWorld();
        var eventWorld = new EcsWorld();
        systems = new EcsSystems(world);
        
        systems
            .AddWorld(eventWorld,EVENT_WORLD)
            .Add(new InitPlayerWithCameraSystem())
            
            .Add(new RegenTickSystem())
            
            .Add(new SpawnEnemyTickSystem())
            .Add(new SpawnEnemySystem())
            .Add(new RangeCrossingSystem())
            .Add(new AttackTickSystem())
            .Add(new UnitAttackSystem())
           
            
            
            .Add(new MoveSystem())
            
            .Add(new ApplyDamageSystem())
            .Add(new SpawnHpViewSystem())
            .Add(new SpawnDamageViewSystem())
            .Add(new CoinGainSystem())
            .Add(new UpdateCoinsViewSystem())
           
            .Add(new DestroyDamagedSystem())
            .Add(new UpdateHpViewSystem())
            .Add(new TickSystem())
            .DelHere<CoinsChangedEventComponent>(EVENT_WORLD)
            .DelHere<ApplyDamageEventComponent>(EVENT_WORLD)
            .DelHere<CreateAttackEventComponent>(EVENT_WORLD)
            .DelHere<EnemySpawnEventComponent>(EVENT_WORLD)
            .DelHere<SpawnHpViewEventComponent>(EVENT_WORLD)
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (EVENT_WORLD))
#endif
            .Inject(new Fabric(world,eventWorld,staticData))
            .Inject(sceneData)
            .Inject(staticData)
            .InjectUgui(sceneData.EcsUguiEmitter,EVENT_WORLD)
            .Init();
        
       
        
    }

    
    void Update()
    {
        systems?.Run();
    }

    private void OnDestroy()
    {
        if (systems!=null)
        {
            systems.Destroy();
            systems = null;
        }

        if (world!=null)
        {
            world.Destroy();
            world = null;
        }
    }
}