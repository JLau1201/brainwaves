using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;

    public void UpdatePlayerName(string playerName) {
        playerNameText.text = playerName;
        gameObject.SetActive(true);
        playerNameText.enabled = true;
    }
}
