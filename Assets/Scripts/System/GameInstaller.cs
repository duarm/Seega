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

    private void OnValidate ()
    {
        if (_board == null)
            _board = FindObjectOfType<Board> ();

        if (_uiManager == null)
            _uiManager = FindObjectOfType<UIManager> ();

        if (_physicsCache == null)
            _physicsCache = FindObjectOfType<PhysicsCache> ();
    }

    public override void InstallBindings ()
    {
        Container.Bind<PhysicsCache> ()
            .FromInstance (_physicsCache)
            .AsSingle ();

        Container.Bind<Board> ()
            .FromInstance (_board)
            .AsSingle ();

        Container.Bind<UIManager> ()
            .FromInstance (_uiManager)
            .AsSingle ();

        Container.Bind<ITurnManager> ()
            .FromInstance (InterfaceFinder.FindObject<ITurnManager> ())
            .AsSingle ();

        Container.Bind<IRayProvider> ()
            .FromInstance (InterfaceFinder.FindObject<IRayProvider> ())
            .AsSingle ();

        Container.Bind<ISelector> ()
            .FromInstance (InterfaceFinder.FindObject<ISelector> ())
            .AsSingle ();

        Container.Bind<IFieldProvider> ()
            .FromInstance (InterfaceFinder.FindObject<IFieldProvider> ())
            .AsSingle ();

        Container.Bind<IPieceProvider> ()
            .FromInstance (InterfaceFinder.FindObject<IPieceProvider> ())
            .AsSingle ();

        Container.Bind<ICaptureVerifier> ()
            .FromInstance (InterfaceFinder.FindObject<ICaptureVerifier> ())
            .AsSingle ();

        Container.Bind<IPhaseManager> ()
            .FromInstance (InterfaceFinder.FindObject<IPhaseManager> ())
            .AsSingle ();

        Container.Bind<IWallVerifier> ()
            .FromInstance (InterfaceFinder.FindObject<IWallVerifier> ())
            .AsSingle ();

        Container.Bind<IGameFinisher> ()
            .FromInstance (InterfaceFinder.FindObject<IGameFinisher> ())
            .AsSingle ();

    }
}