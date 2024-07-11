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

    private int psychicTurnInd;
    private int guesserTurnInd;
    private int teamTurn;


    // Player Info
    private PlayerData playerData;
    private int team;
    private int playerInd;

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

                AssignPlayerRoleClientRpc(teamTurn, psychicTurnInd, guesserTurnInd);

                ChangeTurnState(TurnState.WheelSpin);
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

    [ClientRpc]
    private void AssignPlayerRoleClientRpc(int teamTurn, int psychicTurnInd, int guesserTurnInd) {
        if(team == teamTurn) {
            if (playerInd == psychicTurnInd) {
                TurnManager.Instance.SetPlayerTurnRole(TurnManager.PlayerRole.Psychic);
            } else if (playerInd == guesserTurnInd) {
                TurnManager.Instance.SetPlayerTurnRole(TurnManager.PlayerRole.Guesser);
            } else {
                TurnManager.Instance.SetPlayerTurnRole(TurnManager.PlayerRole.TeamViewer);
            }
        } else {
            TurnManager.Instance.SetPlayerTurnRole(TurnManager.PlayerRole.EnemyViewer);
        }
    }

    private void ChangeGameState(GameState newGameState) {
        currentGameState = newGameState;
        switch (currentGameState) {
            case GameState.Team1Playing:
                teamTurn = 0;
                ChangeTurnState(TurnState.TurnStart);

                break;
            case GameState.Team2Playing:
                teamTurn = 1;

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
    }

    [ClientRpc]
    private void SetPlayerInfoClientRpc(PlayerData playerData, int team, int playerInd) {
        // Assign player data
        this.playerData = MultiplayerManager.Instance.GetPlayerData();

        if(this.playerData.Equals(playerData)) {
            this.team = team;
            this.playerInd = playerInd;
        }
    }

    private void InitializeGame() {
        psychicTurnInd = 0;
        guesserTurnInd = 1;
        teamsList.Add(MultiplayerManager.Instance.GetTeamOnePlayerDataList());
        teamsList.Add(MultiplayerManager.Instance.GetTeamTwoPlayerDataList());
    }
    
    private IEnumerator StartGameCountdown() {
        yield return new WaitForSeconds(gameStartCountdown);

        for (int i = 0; i < teamsList.Count; i++) {
            for (int j = 0; j < teamsList[i].Count; j++) {
                SetPlayerInfoClientRpc(teamsList[i][j], i, j);
            }
        }
        ChangeGameState(GameState.Team1Playing);
    }

    [ClientRpc]
    private void PlayTransitionAnimationClientRpc(string role, string playerName) {
        transitionUI.SetText(role, playerName);

        transitionUI.PlayAnimation();
    }
}
