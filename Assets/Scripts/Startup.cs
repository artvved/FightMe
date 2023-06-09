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
    private EcsWorld eventWorld;
    private EcsSystems systems;
  

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private StaticData staticData;
    void Start()
    {
        world = new EcsWorld();
        eventWorld = new EcsWorld();
        systems = new EcsSystems(world);
        var EVENT_WORLD = Idents.Worlds.EVENT_WORLD;
        
        systems
            .AddWorld(eventWorld,EVENT_WORLD)
            .Add(new InitPlayerWithCameraSystem())
            .Add(new SpellButtonsInputCheckSystem())
            
            .Add(new RegenTickSystem())
            
            .Add(new SpawnEnemyTickSystem())
            .Add(new SpawnEnemySystem())
            
            .Add(new TargetingClosestSystem())
            
            .Add(new MoveToTargetSystem())
            .Add(new MoveToPositionSystem())
            .Add(new MoveApplySystem())
            
            .Add(new RangeCrossingSystem())
            
            .Add(new CreateAttackTickSystem())
            .Add(new ChainLightningAttackSystem())
            .Add(new MindControlAttackSystem())
            .Add(new MeteorAttackSystem())
            .Add(new BloodlustIncreaseAttackSystem())
            .Add(new LifestealSystem())
            .Add(new TargetingOffenderSystem())
            
            .Add(new CreateDamageSystem())
            .Add(new ApplyDamageSystem())
            
            .Add(new CoinGainSystem())
            .Add(new GameOverDetectionSystem())
            .Add(new GameOverUIPopupSystem())
            .Add(new RestartGameSystem())
            
            .Add(new SpawnHpViewSystem())
            .Add(new SpawnDamageViewSystem())
            .Add(new UpdateCoinsViewSystem())
            .Add(new LifetimeSystem())
            .Add(new TickSystem())
            .Add(new DestroyDamagedSystem())
            
            .Add(new UpdateHpViewSystem())
            .Add(new UpdateSpellButtonsCDSystem())
            
            //spells
            .DelHere<ChainLightningSpellEventComponent>(EVENT_WORLD)
            .DelHere<MindControlSpellEventComponent>(EVENT_WORLD)
            .DelHere<MeteorSpellEventComponent>(EVENT_WORLD)
            
            .DelHere<GameOverEventComponent>(EVENT_WORLD)
            .DelHere<CoinsChangedEventComponent>(EVENT_WORLD)
            .DelHere<ApplyDamageEventComponent>(EVENT_WORLD)
            .DelHere<CreateAttackEventComponent>(EVENT_WORLD)
            .DelHere<EnemySpawnEventComponent>(EVENT_WORLD)
            .DelHere<SpawnHpViewEventComponent>(EVENT_WORLD)
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (EVENT_WORLD))
#endif
            .Inject(new Fabric(world,eventWorld,staticData,sceneData))
            .Inject(sceneData)
            .Inject(staticData)
            .Inject(new PositionService(world))
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
        
        if (eventWorld!=null)
        {
            eventWorld.Destroy();
            eventWorld = null;
        }
    }
}
