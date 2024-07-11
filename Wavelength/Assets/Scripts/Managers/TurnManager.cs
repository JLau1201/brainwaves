using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public static TurnManager Instance { get; private set; }

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
    }

    public void ChangeTurn() {
        switch (currentPlayerRole) {
            case PlayerRole.Psychic:
                break;
            case PlayerRole.Guesser:

                break;
            case PlayerRole.TeamViewer:

                break;
            case PlayerRole.EnemyViewer:

                break;
        }
    }
}
