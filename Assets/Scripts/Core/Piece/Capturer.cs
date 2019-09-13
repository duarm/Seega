using Kurenaiz.Utilities.Pooling;
using UnityEngine;
using Zenject;

public class Capturer : MonoBehaviour, ICapturer
{
    private VFXController _vFXController;

    [Inject]
    private void Construct(VFXController vFXController)
    {
        _vFXController = vFXController;
    }

    public void Capture ()
    {
        _vFXController.Trigger("SmokeParticle", transform.position, 0, false, null);
        Destroy (this.gameObject, .5f);
    }
}