using UnityEngine;
using System.Collections.Generic;
using System;
using Seega.GlobalEnums;

namespace Kurenaiz.Utilities.Events
{
    public class EventManager : MonoBehaviour {

        private Dictionary <string, Action> eventDictionary;

        public Action<GameState> OnStateChange;
        public Action<bool> OnTurnChange;
        public Action<string, string> OnGameEnd;

        void Init ()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, Action>();
            }
        }

        public void StartListening (string eventName, Action listener)
        {
            Action thisEvent;
            if (eventDictionary.TryGetValue (eventName, out thisEvent))
            {
                thisEvent += listener;
                eventDictionary[eventName] = thisEvent;
            } 
            else
            {
                thisEvent += listener;
                eventDictionary.Add(eventName, thisEvent);
            }
        }

        public void StopListening (string eventName, Action listener)
        {
            Action thisEvent = null;
            if (eventDictionary.TryGetValue (eventName, out thisEvent))
            {
                thisEvent -= listener;
                eventDictionary[eventName] = thisEvent;
            }
        }

        public void TriggerEvent (string eventName)
        {
            Action thisEvent = null;
            if (eventDictionary.TryGetValue (eventName, out thisEvent))
            {
                thisEvent.Invoke ();
            }
        }
    }
}