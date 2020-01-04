using Kurenaiz.Utilities.Audio;
using Kurenaiz.Utilities.Events;
using Kurenaiz.Utilities.Pooling;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] ScreenFader _screenFader;
    [SerializeField] SceneController _sceneController;
    [SerializeField] EventManager _eventManager;
    [SerializeField] VFXController _vfxController;
    [SerializeField] BackgroundMusicPlayer _backgroundMusicPlayer;

    private void OnValidate ()
    {
        if (_screenFader == null)
            _screenFader = GetComponentInChildren<ScreenFader> ();

        if (_sceneController == null)
            _sceneController = GetComponentInChildren<SceneController> ();

        if (_eventManager == null)
            _eventManager = GetComponentInChildren<EventManager> ();

        if (_vfxController == null)
            _vfxController = GetComponentInChildren<VFXController> ();

        if (_backgroundMusicPlayer == null)
            _backgroundMusicPlayer = GetComponentInChildren<BackgroundMusicPlayer> ();
    }

    public override void InstallBindings ()
    {
        Container.Bind<ScreenFader> ()
            .FromInstance (_screenFader)
            .AsSingle ();

        Container.Bind<SceneController> ()
            .FromInstance (_sceneController)
            .AsSingle ();

        Container.Bind<EventManager> ()
            .FromInstance (_eventManager)
            .AsSingle ();

        Container.Bind<VFXController> ()
            .FromInstance (_vfxController)
            .AsSingle ();

        Container.Bind<BackgroundMusicPlayer> ()
            .FromInstance (_backgroundMusicPlayer)
            .AsSingle ();
    }
}