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

    private void OnValidate() {
        if (_board == null)
            _board = FindObjectOfType<Board>();
        
        if(_uiManager == null)
            _uiManager = FindObjectOfType<UIManager>();
        
        if(_physicsCache == null)
            _physicsCache = FindObjectOfType<PhysicsCache>();
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

    }
}