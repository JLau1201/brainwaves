using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WavelengthManager : NetworkBehaviour
{
    public static WavelengthManager Instance { get; private set; }


    [Header("Scripts")]
    [SerializeField] private TransitionUI transitionUI;

    //Testing
    [SerializeField] private Button button;

    private int psychicTurnInd;
    private int guesserTurnInd;
    private int teamTurn;

    private List<List<PlayerData>> teamsList = new List<List<PlayerData>>();

    private float gameStartCountdown = 3f;

    // Track current state of the game
    private GameState currentGameState;    
    public enum GameState {
        Team1Playing,
        Team2Playing,
        Finished,
    }

    // Track current turn state
    private TurnState currentTurnState;
    public enum TurnState {
        TurnStart,
        WheelSpin,
        Psychic,
        Guesser,
        TurnEnd,
    }

    private void Start() {
        transitionUI.OnTransitionAnimationFinished += TurnStartUIScript_OnTurnStartAnimationFinished;
    }

    private void TurnStartUIScript_OnTurnStartAnimationFinished(object sender, System.EventArgs e) {
        switch (currentTurnState) {
            case TurnState.TurnStart:
                break;
        }
    }

    private void ChangeTurnState(TurnState newTurnState) {
        currentTurnState = newTurnState;
        switch (currentTurnState) {
            case TurnState.TurnStart:
                string role = "Psychic";
                string playerName = teamsList[teamTurn][psychicTurnInd].playerName.ToString();
                PlayTransitionAnimationClientRpc(role, playerName);

                break;
            case TurnState.WheelSpin:
                break;
            case TurnState.Psychic:
                break;
            case TurnState.Guesser:
                break;
            case TurnState.TurnEnd:
                break;
        }
    }

    private void ChangeGameState(GameState newGameState) {
        currentGameState = newGameState;
        switch (currentGameState) {
            case GameState.Team1Playing:
                ChangeTurnState(TurnState.TurnStart);

                break;
            case GameState.Team2Playing:
                break;
            case GameState.Finished:
                break;
        }
    }

    private void Awake() {
        Instance = this;
        if (NetworkManager.Singleton.IsHost) {
            InitializeGame();
            StartCoroutine(StartGameCountdown());
        }

        button.onClick.AddListener(() => {
        });
    }
    
    private void InitializeGame() {
        psychicTurnInd = 0;
        guesserTurnInd = 1;
        teamTurn = 0;
        teamsList.Add(MultiplayerManager.Instance.GetTeamOnePlayerDataList());
        teamsList.Add(MultiplayerManager.Instance.GetTeamTwoPlayerDataList());
    }
    
    private IEnumerator StartGameCountdown() {
        yield return new WaitForSeconds(gameStartCountdown);

        ChangeGameState(GameState.Team1Playing);
    }

    [ClientRpc]
    private void PlayTransitionAnimationClientRpc(string role, string playerName) {
        transitionUI.SetText(role, playerName);

        transitionUI.PlayAnimation();
    }
}
