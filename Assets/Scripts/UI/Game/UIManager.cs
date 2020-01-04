using Kurenaiz.Utilities.Events;
using Seega.Enums;
using TMPro;
using UnityEngine;
using Zenject;

namespace Seega.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI turnText;
        [SerializeField] TextMeshProUGUI stateText;

        [SerializeField] TextMeshProUGUI winnerText;
        [SerializeField] TextMeshProUGUI victoryTypeText;

        [SerializeField] GameObject endGameWindow;
        [SerializeField] GameObject surrenderButton;

        [SerializeField]

        readonly string[] turns = new string[2] { "Branca", "Preta" };
        readonly string[] states = new string[2] { "Posicionamento", "Movimento" };

        private Board _board;
        private SceneController _sceneController;
        private EventManager _eventManager;

        [Inject]
        private void Construct (Board board,
            SceneController sceneController,
            EventManager eventManager)
        {
            _board = board;
            _sceneController = sceneController;
            _eventManager = eventManager;
        }

        private void Start ()
        {
            _eventManager.OnTurnChange += UpdateTurnUI;
            _eventManager.OnStateChange += UpdateStateUI;
            _eventManager.OnGameEnd += ActivateEndWindow;
        }

        private void OnDestroy()
        {
            _eventManager.OnTurnChange -= UpdateTurnUI;
            _eventManager.OnStateChange -= UpdateStateUI;
            _eventManager.OnGameEnd -= ActivateEndWindow;
        }

        public void RestartButton ()
        {
            _sceneController.RestartScene ();
        }

        public void ActivateEndWindow (ColorType winner, string winType)
        {
            surrenderButton.SetActive (false);
            endGameWindow.SetActive (true);
            winnerText.text = winner == ColorType.BLACK ? "Pretas" : "Brancas";
            victoryTypeText.text = winType;
        }

        public void UpdateTurnUI (bool isWhiteTurn)
        {
            if (isWhiteTurn)
                turnText.text = turns[0];
            else
                turnText.text = turns[1];
        }

        public void UpdateStateUI (Phase state)
        {
            if (state == Phase.POSITIONING)
                stateText.text = states[0];
            else if (state == Phase.MOVEMENT)
            {
                surrenderButton.SetActive (true);
                stateText.text = states[1];
            }
        }

        //Called by the Help Button
        public void InvertActive (GameObject objectToInvert)
        {
            objectToInvert.SetActive (!objectToInvert.activeInHierarchy);
        }
    }
}