using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WavelengthManager : NetworkBehaviour
{
    public static WavelengthManager Instance { get; private set; }

    //Testing
    [SerializeField] private Button button;

    // Track current state of the game
    private GameState currentGameState;    
    public enum GameState {
        Countdown,
        Team1Playing,
        Team2Playing,
        Paused,
        Finished,
    }

    private void Awake() {
        Instance = this;
        currentGameState = GameState.Team1Playing;

        button.onClick.AddListener(() => {
            IReadOnlyList<ulong> connectedClients = NetworkManager.Singleton.ConnectedClientsIds;
            foreach (ulong client in connectedClients) {
                Debug.Log(client);
            }
        });
    }

    private void Update() {
    }

    private void ChangeGameState(GameState newGameState) {
        currentGameState = newGameState;

        switch(currentGameState) {
            case GameState.Countdown:
                break;
            case GameState.Team1Playing:
                break;
            case GameState.Team2Playing:
                break;
            case GameState.Paused:
                break;
            case GameState.Finished:
                break;
            default:
                Debug.LogError("Unknown Game State!");
                break;
        }
    }
}
