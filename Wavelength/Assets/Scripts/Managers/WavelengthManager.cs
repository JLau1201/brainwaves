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
    [SerializeField] private TeamUI teamOneUI;
    [SerializeField] private TeamUI teamTwoUI;
    [SerializeField] private AddScoreAnimation addScoreTeamOneAnimation;
    [SerializeField] private AddScoreAnimation addScoreTeamTwoAnimation;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private ParticleSystem confetti;

    [Header("Buttons")]
    [SerializeField] private Button confirmButton;

    private int teamOnePsychicTurnInd;
    private int teamOneGuesserTurnInd;
    private int teamTwoPsychicTurnInd;
    private int teamTwoGuesserTurnInd;
    private int teamTurn;

    // Player Info
    private PlayerData playerData;
    private int team;
    private int playerInd;

    private List<List<PlayerData>> teamsList = new List<List<PlayerData>>();
    private int teamOneScore;
    private int teamTwoScore;

    private float gameStartCountdown = 3f;

    private int targetScore = 10;
    private bool isGameStarted = false;

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
        addScoreTeamOneAnimation.OnAddScoreAnimationFinished += AddScoreTeamOneAnimation_OnAddScoreAnimationFinished;
        addScoreTeamTwoAnimation.OnAddScoreAnimationFinished += AddScoreTeamTwoAnimation_OnAddScoreAnimationFinished;
    }

    private void AddScoreTeamTwoAnimation_OnAddScoreAnimationFinished(object sender, System.EventArgs e) {
        teamTwoUI.UpdateScore(teamTwoScore);
        if (teamTwoScore >= targetScore) {
            PlayConfettiClientRpc();
            PlayTransitionAnimationClientRpc("", "2");
        }
    }


    private void AddScoreTeamOneAnimation_OnAddScoreAnimationFinished(object sender, System.EventArgs e) {
        teamOneUI.UpdateScore(teamOneScore);
        if (teamOneScore >= targetScore) {
            PlayConfettiClientRpc();
            PlayTransitionAnimationClientRpc("", "1");
        }
    }

    [ClientRpc]
    private void PlayConfettiClientRpc() {
        confetti.Play();
    }

    private void WheelSpinAnimation_OnWheelSpinFinished(object sender, System.EventArgs e) {
        ChangeTurnState(TurnState.Psychic);
    }

    private void TransitionUIScript_OnTransitionAnimationFinished(object sender, System.EventArgs e) {
        if (!NetworkManager.Singleton.IsHost) return;
        if (!isGameStarted) {
            isGameStarted = true;
            return;
        }
        if (currentGameState == GameState.Finished) return;
        switch (currentTurnState) {
            case TurnState.TurnStart:
                cardDisplay.ChooseRandomCard();

                ChangeTurnState(TurnState.WheelSpin);
                break;
            case TurnState.Psychic:
                ChangeTurnState(TurnState.Guesser);
                break;
            case TurnState.TurnEnd:
                StartCoroutine(StartNewTurn());
                break;
        }
    }

    private void ChangeTurnState(TurnState newTurnState) {
        currentTurnState = newTurnState;
        switch (currentTurnState) {
            case TurnState.TurnStart:
                string role = "Psychic";
                string playerName;
                if (teamTurn == 0) {
                    playerName = teamsList[teamTurn][teamOnePsychicTurnInd].playerName.ToString();
                    AssignPlayerRoleClientRpc(teamTurn, teamOnePsychicTurnInd, teamOneGuesserTurnInd);
                    rotateDial.ChangeOwnership(teamsList[teamTurn][teamOneGuesserTurnInd].clientId);
                } else { 
                    playerName = teamsList[teamTurn][teamTwoPsychicTurnInd].playerName.ToString();
                    AssignPlayerRoleClientRpc(teamTurn, teamTwoPsychicTurnInd, teamTwoGuesserTurnInd);
                    rotateDial.ChangeOwnership(teamsList[teamTurn][teamTwoGuesserTurnInd].clientId);
                }

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
                if(teamOneScore >= targetScore || teamTwoScore >= targetScore) {
                    ChangeGameState(GameState.Finished);
                }
                ChangeTurnClientRpc(TurnState.TurnEnd);
                StartCoroutine(EndOfTurn());
                break;
        }
    }

    private IEnumerator EndOfTurn() {
        int overStateTime;
        if(currentGameState == GameState.Finished) {
            overStateTime = 10;
            yield return new WaitForSeconds(overStateTime);
            ShowGameOverUIClientRpc();
        } else {
            overStateTime = 3;
            yield return new WaitForSeconds(overStateTime);

            string role = "Changing Sides";
            switch (teamTurn) {
                case 0:
                    if (teamsList[1].Count == 0) {
                        PlayTransitionAnimationClientRpc(role, MultiplayerManager.Instance.GetTeamOneName());
                    } else {
                        PlayTransitionAnimationClientRpc(role, MultiplayerManager.Instance.GetTeamTwoName());
                    }
                    break;
                case 1:
                    PlayTransitionAnimationClientRpc(role, MultiplayerManager.Instance.GetTeamOneName());
                    break;
            }
        }
    }

    [ClientRpc]
    private void ShowGameOverUIClientRpc() {
        gameOverUI.Show();
    }

    private IEnumerator StartNewTurn() {
        int waitTime = 1;
        yield return new WaitForSeconds(waitTime);

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
                teamOnePsychicTurnInd = (teamOnePsychicTurnInd + 1) % teamsList[0].Count;
                teamOneGuesserTurnInd = (teamOneGuesserTurnInd + 1) % teamsList[0].Count;

                ChangeTurnState(TurnState.TurnStart);
                break;
            case GameState.Team2Playing:
                teamTurn = 1;

                teamTwoPsychicTurnInd = (teamTwoPsychicTurnInd + 1) % teamsList[1].Count;
                teamTwoGuesserTurnInd = (teamTwoGuesserTurnInd + 1) % teamsList[1].Count;

                ChangeTurnState(TurnState.TurnStart);
                break;
            case GameState.Finished:
                break;
        }
    }

    private void Awake() {
        Instance = this;
        confetti.Stop();
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
                string playerName = teamsList[teamTurn][teamOneGuesserTurnInd].playerName.ToString();

                PlayTransitionAnimationClientRpc(role, playerName);
                break;
            case TurnState.Guesser:
                int score = confirmDial.GetGuessScore();
                switch (teamTurn) {
                    case 0:
                        PlayAddScoreTeamOneAnimationClientRpc(score);
                        teamOneScore += score;
                        break;
                    case 1:
                        PlayAddScoreTeamTwoAnimationClientRpc(score);
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
        teamOnePsychicTurnInd = -1;
        teamOneGuesserTurnInd = 0;
        teamTwoPsychicTurnInd = -1;
        teamTwoGuesserTurnInd = 0;
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
    
    [ClientRpc]
    private void PlayAddScoreTeamOneAnimationClientRpc(int newScore) {
        addScoreTeamOneAnimation.SetText(newScore);

        addScoreTeamOneAnimation.PlayAnimation();
    }
    
    [ClientRpc]
    private void PlayAddScoreTeamTwoAnimationClientRpc(int newScore) {
        addScoreTeamTwoAnimation.SetText(newScore);

        addScoreTeamTwoAnimation.PlayAnimation();
    }
}
