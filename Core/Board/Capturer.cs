using UnityEngine;

namespace Seega.Scripts.Core
{
    public class Capturer : MonoBehaviour, ICapturer
    {
        ParticleSystem _deathParticle;

        private void Start()
        {
            _deathParticle = GetComponentInChildren<ParticleSystem> ();
        }

        public void Capture()
        {
            _deathParticle.Play ();
            Destroy (this.gameObject, .5f);
        }
    }
}