using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    public static TurnManager Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button spinButton;

    [Header("Scripts")]
    [SerializeField] private WheelSpinAnimation wheelSpinAnimation;
    [SerializeField] private WheelCover wheelCover;
    [SerializeField] private RotateDial rotateDial;

    // Track the role of the player during the turn
    private PlayerRole currentPlayerRole;
    public enum PlayerRole {
        Psychic,
        Guesser,
        TeamViewer,
        EnemyViewer,
    }

    private void Awake() {
        Instance = this;
    }

    public void SetPlayerTurnRole(PlayerRole role) {
        currentPlayerRole = role;
        wheelCover.CloseCover();
    }

    // FIX: Team 1 Client sometimes doesnt opencover? unknown reason
    public void WheelSpinTurn() {
        switch (currentPlayerRole) {
            case PlayerRole.Psychic:
                wheelCover.OpenCover();
                spinButton.enabled = true;
                break;
            case PlayerRole.EnemyViewer:
                wheelCover.OpenCover();
                break;
        }
    }

    public void PsychicTurn() {
        switch (currentPlayerRole) {
            case PlayerRole.Psychic:
                confirmButton.interactable = true;
                break;
        }
    }

    public void GuesserTurn() {
        switch (currentPlayerRole) {
            case PlayerRole.Psychic:
                wheelCover.CloseCover();
                confirmButton.interactable = false;
                break;
            case PlayerRole.Guesser:
                Debug.Log("YOUR GUESS TURN");
                rotateDial.SetCanSpin(true);
                confirmButton.interactable = true;
                break;
            case PlayerRole.EnemyViewer:
                wheelCover.CloseCover();
                break;
        }
    }

    public void TurnOver() {
        wheelCover.OpenCover();
        switch (currentPlayerRole) {
            case PlayerRole.Guesser:
                rotateDial.SetCanSpin(false);
                confirmButton.interactable = false;
                break;
        }
    }
}
