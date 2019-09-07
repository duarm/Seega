using Kurenaiz.Utilities.Events;
using Kurenaiz.Utilities.Physics;
using Seega.Scripts.Core;
using Seega.Scripts.UI;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] Board _board;
    [SerializeField] UIManager _uiManager;
    [SerializeField] PhysicsCache _physicsCache;
    [SerializeField] ScreenFader _screenFader;
    [SerializeField] SceneController _sceneController;
    [SerializeField] EventManager _eventManager;

    private void OnValidate() {
        if (_board == null)
            _board = FindObjectOfType<Board>();
        
        if(_uiManager == null)
            _uiManager = FindObjectOfType<UIManager>();
        
        if(_physicsCache == null)
            _physicsCache = FindObjectOfType<PhysicsCache>();
        
        if(_screenFader == null)
            _screenFader = FindObjectOfType<ScreenFader>();

        if(_sceneController == null)
            _sceneController = FindObjectOfType<SceneController>();
        
        if(_eventManager == null)
            _eventManager = FindObjectOfType<EventManager>();
    }

    public override void InstallBindings()
    {
        Container.Bind<PhysicsCache>()
            .FromInstance(_physicsCache)
            .AsSingle();

        Container.Bind<Board>()
            .FromInstance(_board)
            .AsSingle();

        Container.Bind<UIManager>()
            .FromInstance(_uiManager)
            .AsSingle();

        Container.Bind<ScreenFader>()
            .FromInstance(_screenFader)
            .AsSingle();

        Container.Bind<SceneController>()
            .FromInstance(_sceneController)
            .AsSingle();

        Container.Bind<EventManager>()
            .FromInstance(_eventManager)
            .AsSingle();
    }
}