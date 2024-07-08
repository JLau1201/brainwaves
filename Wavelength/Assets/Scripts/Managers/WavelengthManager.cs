using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WavelengthManager : MonoBehaviour
{
    public static WavelengthManager Instance { get; private set; }

    List<int> Team1 = new List<int>();
    List<int> Team2 = new List<int>();

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
}
