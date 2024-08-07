using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TeamUI : NetworkBehaviour
{
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI teamNameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Transforms")]
    [SerializeField] private Transform playerDataHolder;
    [SerializeField] private Transform playerDataSingle;



    [SerializeField] private int team;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        switch (team) {
            case 1:
                SetTeamName(MultiplayerManager.Instance.GetTeamOneName());
                List<PlayerData> teamOne = MultiplayerManager.Instance.GetTeamOnePlayerDataList();

                foreach (PlayerData playerData in teamOne) {
                    PopulatePlayerDataHolderClientRpc(playerData.playerName.ToString());
                }
                break;
            case 2:
                SetTeamName(MultiplayerManager.Instance.GetTeamTwoName());
                List<PlayerData> teamTwo = MultiplayerManager.Instance.GetTeamTwoPlayerDataList();

                foreach (PlayerData playerData in teamTwo) {
                    PopulatePlayerDataHolderClientRpc(playerData.playerName.ToString());
                }
                break;
        }
    }

    public void SetTeamName(string teamName) {
        teamNameText.text = teamName;
    }

    public void UpdateScore(int score) {
        UpdateScoreClientRpc(score);
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int score) {
        scoreText.text = score.ToString();
    }

    [ClientRpc]
    private void PopulatePlayerDataHolderClientRpc(string playerName) {
        Transform playerDataSingleTransform = Instantiate(playerDataSingle, playerDataHolder);
        PlayerDataSingleUI playerDataSingleUI = playerDataSingleTransform.gameObject.GetComponent<PlayerDataSingleUI>();
        playerDataSingleUI.UpdatePlayerName(playerName);
    }

    public void UpdatePlayerIcons(int psychicInd, int guesserInd) {
        UpdatePlayerIconsClientRpc(psychicInd, guesserInd);
    }

    [ClientRpc]
    private void UpdatePlayerIconsClientRpc(int psychicInd, int guesserInd) {
        int i = 0;
        foreach(Transform child in playerDataHolder) {
            if (child.gameObject.activeSelf == false) continue;
            PlayerDataSingleUI playerDataSingleUI = child.GetComponent<PlayerDataSingleUI>();
            if(i == psychicInd) {
                playerDataSingleUI.SetPlayerIcon(0);
            }else if(i == guesserInd) {
                playerDataSingleUI.SetPlayerIcon(1);
            } else {
                playerDataSingleUI.SetPlayerIcon(2);
            }
            i++;
        }
    }
}
