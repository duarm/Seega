using System;

namespace Kurenaiz.Utilities.Events
{
    public interface IEventManager
    {
        void StartListening (string eventName, Action listener);
        void StopListening (string eventName, Action listener);
        void TriggerEvent (string eventName);
    }
}