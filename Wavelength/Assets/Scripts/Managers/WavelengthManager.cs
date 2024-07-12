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
    [SerializeField] private WheelSpinAnimation wheelSpinAnimation;
    [SerializeField] private ConfirmDial confirmDial;
    [SerializeField] private RotateDial rotateDial;
    [SerializeField] private CardDisplay cardDisplay;

    [Header("Buttons")]
    [SerializeField] private Button confirmButton;

    // *** TODO ***
    // TEAM CHANGE ANIMATION/STALL DURATION
    // PSYCHIC/GUESSER TURN IND ROTATION
    // IN GAME UI
    // ^- SCORE/TEAM NAME/PLAYER DATA/ICON
    // GAME OVER - 10 POINTS


    private int psychicTurnInd;
    private int guesserTurnInd;
    private int teamTurn;

    // Player Info
    private PlayerData playerData;
    private int team;
    private int playerInd;

    private List<List<PlayerData>> teamsList = new List<List<PlayerData>>();
    private int teamOneScore;
    private int teamTwoScore;

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
        transitionUI.OnTransitionAnimationFinished += TransitionUIScript_OnTransitionAnimationFinished;
        wheelSpinAnimation.OnWheelSpinFinished += WheelSpinAnimation_OnWheelSpinFinished;
    }

    private void WheelSpinAnimation_OnWheelSpinFinished(object sender, System.EventArgs e) {
        ChangeTurnState(TurnState.Psychic);
    }

    private void TransitionUIScript_OnTransitionAnimationFinished(object sender, System.EventArgs e) {
        if (!NetworkManager.Singleton.IsHost) return;
        switch (currentTurnState) {
            case TurnState.TurnStart:
                cardDisplay.ChooseRandomCard();

                ChangeTurnState(TurnState.WheelSpin);
                break;
            case TurnState.Psychic:
                ChangeTurnState(TurnState.Guesser);
                break;
        }
    }

    private void ChangeTurnState(TurnState newTurnState) {
        currentTurnState = newTurnState;
        switch (currentTurnState) {
            case TurnState.TurnStart:
                string role = "Psychic";
                string playerName = teamsList[teamTurn][psychicTurnInd].playerName.ToString();
                AssignPlayerRoleClientRpc(teamTurn, psychicTurnInd, guesserTurnInd);
                rotateDial.ChangeOwnership(teamsList[teamTurn][guesserTurnInd].clientId);
                PlayTransitionAnimationClientRpc(role, playerName);
                break;
            case TurnState.WheelSpin:
                ChangeTurnClientRpc(currentTurnState);
                break;
            case TurnState.Psychic:
                ChangeTurnClientRpc(currentTurnState);
                break;
            case TurnState.Guesser:
                ChangeTurnClientRpc(currentTurnState);
                break;
            case TurnState.TurnEnd:
                ChangeTurnClientRpc(TurnState.TurnEnd);

                // Stall/Add transition to next teams turn
                switch (teamTurn) {
                    case 0:
                        if (teamsList[1].Count == 0) {
                            ChangeGameState(GameState.Team1Playing);
                        } else {
                            ChangeGameState(GameState.Team2Playing);
                        }
                        break;
                    case 1:
                        ChangeGameState(GameState.Team1Playing);
                        break;
                }

                break;
        }
    }

    [ClientRpc]
    private void ChangeTurnClientRpc(TurnState turnState) {
        currentTurnState = turnState;
        switch (currentTurnState) {
            case TurnState.TurnStart:
                break;
            case TurnState.WheelSpin:
                TurnManager.Instance.WheelSpinTurn();
                break;
            case TurnState.Psychic:
                TurnManager.Instance.PsychicTurn();
                break;
            case TurnState.Guesser:
                TurnManager.Instance.GuesserTurn();
                break;
            case TurnState.TurnEnd:
                TurnManager.Instance.TurnOver();
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
                
                ChangeTurnState(TurnState.TurnStart);
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

        confirmButton.onClick.AddListener(OnConfirmButtonServerRpc);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnConfirmButtonServerRpc() {
        switch (currentTurnState) {
            case TurnState.Psychic:
                string role = "Guesser";
                string playerName = teamsList[teamTurn][guesserTurnInd].playerName.ToString();

                PlayTransitionAnimationClientRpc(role, playerName);
                break;
            case TurnState.Guesser:
                int score = confirmDial.GetGuessScore();
                switch (teamTurn) {
                    case 0:
                        teamOneScore += score;
                        break;
                    case 1:
                        teamTwoScore += score;
                        break;
                }

                ChangeTurnState(TurnState.TurnEnd);
                break;
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
