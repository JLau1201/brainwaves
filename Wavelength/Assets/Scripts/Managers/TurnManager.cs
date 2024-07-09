using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    // Track the role of the player during the turn
    private PlayerRole currentPlayerRole;
    public enum PlayerRole {
        Psychic,
        Guesser,
        TeamViewer,
        EnemyViewer,
    }


}
