using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDataSingleUI : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI guesserIcon;

    [Header("Images")]
    [SerializeField] private Image psychicIcon;

    public void UpdatePlayerName(string playerName) {
        playerNameText.text = playerName;
        gameObject.SetActive(true);
        playerNameText.enabled = true;
    }

    public void SetPlayerIcon(int role) {
        switch (role) {
            case 0:
                psychicIcon.gameObject.SetActive(true);
                guesserIcon.gameObject.SetActive(false);
                break;
            case 1:
                psychicIcon.gameObject.SetActive(false);
                guesserIcon.gameObject.SetActive(true);
                break;
            case 2:
                psychicIcon.gameObject.SetActive(false);
                guesserIcon.gameObject.SetActive(false);
                break;
        }
    }
}
