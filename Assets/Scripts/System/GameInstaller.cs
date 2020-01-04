using Kurenaiz.Utilities.Events;
using Kurenaiz.Utilities.Interfaces;
using Kurenaiz.Utilities.Physics;
using Seega.UI;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] Board board;
    [SerializeField] UIManager uiManager;
    [SerializeField] PhysicsCache physicsCache;
    [SerializeField] EventManager eventManager;

    private void OnValidate ()
    {
        if (board == null)
            board = FindObjectOfType<Board> ();

        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager> ();

        if (physicsCache == null)
            physicsCache = FindObjectOfType<PhysicsCache> ();

        if (eventManager == null)
            eventManager = FindObjectOfType<EventManager> ();
    }

    public override void InstallBindings ()
    {
        Container.Bind<PhysicsCache> ()
            .FromInstance (physicsCache)
            .AsSingle ();

        Container.Bind<Board> ()
            .FromInstance (board)
            .AsSingle ();

        Container.Bind<UIManager> ()
            .FromInstance (uiManager)
            .AsSingle ()
            .NonLazy ();

        Container.Bind<EventManager> ()
            .FromInstance (eventManager)
            .AsSingle ();

        Container.Bind<ITurnManager> ()
            .FromInstance (InterfaceFinder.FindObject<ITurnManager> ())
            .AsSingle ();

        Container.Bind<IRayProvider> ()
            .FromInstance (InterfaceFinder.FindObject<IRayProvider> ())
            .AsSingle ();

        ISelector selector = null;
        if (selector != null)
            Container.Bind<ISelector> ()
            .FromInstance (selector)
            .AsSingle ();

        var selector2d = InterfaceFinder.FindObject<ISelector2D> ();
        if (selector2d != null)
            Container.Bind<ISelector2D> ()
            .FromInstance (selector2d)
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

        Container.Bind<IMovementValidator> ()
            .FromInstance (InterfaceFinder.FindObject<IMovementValidator> ())
            .AsSingle ();

    }
}