using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerDataSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void UpdatePlayerName(string playerName) {
        playerNameText.text = playerName;
        playerNameText.enabled = true;
    }
}
