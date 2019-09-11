using Kurenaiz.Utilities.Events;
using Kurenaiz.Utilities.Interfaces;
using Kurenaiz.Utilities.Physics;
using Seega.UI;
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
    [SerializeField] IRayProvider _rayProvider;
    [SerializeField] ISelector _selector;
    [SerializeField] IFieldProvider _fieldProvider;
    [SerializeField] IPieceProvider _pieceProvider;
    [SerializeField] ICaptureVerifier _captureVerifier;
    [SerializeField] ITurnManager _turnManager;
    [SerializeField] IPhaseManager _phaseManager;
    [SerializeField] IWallVerifier _wallVerifier;
    [SerializeField] IGameFinisher _gameFinisher;

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

        Container.Bind<ITurnManager>()
            .FromInstance(InterfaceFinder.FindObject<ITurnManager>())
            .AsSingle();

        Container.Bind<IRayProvider>()
            .FromInstance(InterfaceFinder.FindObject<IRayProvider>())
            .AsSingle();

        Container.Bind<ISelector>()
            .FromInstance(InterfaceFinder.FindObject<ISelector>())
            .AsSingle();

        Container.Bind<IFieldProvider>()
            .FromInstance(InterfaceFinder.FindObject<IFieldProvider>())
            .AsSingle();

        Container.Bind<IPieceProvider>()
            .FromInstance(InterfaceFinder.FindObject<IPieceProvider>())
            .AsSingle();

        Container.Bind<ICaptureVerifier>()
            .FromInstance(InterfaceFinder.FindObject<ICaptureVerifier>())
            .AsSingle();

        Container.Bind<IPhaseManager>()
            .FromInstance(InterfaceFinder.FindObject<IPhaseManager>())
            .AsSingle();

        Container.Bind<IWallVerifier>()
            .FromInstance(InterfaceFinder.FindObject<IWallVerifier>())
            .AsSingle();
            
        Container.Bind<IGameFinisher>()
            .FromInstance(InterfaceFinder.FindObject<IGameFinisher>())
            .AsSingle();
        
    }
}