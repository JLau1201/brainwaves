using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : BaseUI
{
    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button joinTeamOneButton;
    [SerializeField] private Button joinTeamTwoButton;

    [Header("UIs")]
    [SerializeField] private CreateJoinUI createJoinUI;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField lobbyCode;
    [SerializeField] private TMP_InputField TeamOneNameInputField;
    [SerializeField] private TMP_InputField TeamTwoNameInputField;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI teamOneSizeText;
    [SerializeField] private TextMeshProUGUI teamTwoSizeText;

    [Header("Transforms")]
    [SerializeField] private Transform teamOnePlayerDataHodler;
    [SerializeField] private Transform teamTwoPlayerDataHodler;
    [SerializeField] private Transform playerDataSingle;

    private void Start() {
        LobbyManager.Instance.OnLobbyUpdated += LobbyManager_OnLobbyUpdated;
        MultiplayerManager.Instance.OnTeamJoined += MultiplayerManager_OnTeamJoined;

        Hide();
    }

    private void MultiplayerManager_OnTeamJoined(object sender, System.EventArgs e) {
        List<PlayerData> teamOneList = MultiplayerManager.Instance.GetTeamOnePlayerDataList();
        int teamOneSize = teamOneList.Count;
        List<PlayerData> teamTwoList = MultiplayerManager.Instance.GetTeamTwoPlayerDataList();
        int teamTwoSize = teamTwoList.Count;

        UpdateTeamDisplayClientRpc(teamOneSize, teamTwoSize);

        foreach (PlayerData playerData in teamOneList) {
            PopulateTeamOnePlayerDataHolderClientRpc(playerData.playerName);
        }

        foreach (PlayerData playerData in teamTwoList) {
            PopulateTeamTwoPlayerDataHolderClientRpc(playerData.playerName);
        }
    }

    [ClientRpc]
    private void UpdateTeamDisplayClientRpc(int teamOneSize, int teamTwoSize) {
        // Team one
        foreach(Transform child in teamOnePlayerDataHodler) {
            if (child == playerDataSingle) continue;
            Destroy(child.gameObject);
        }

        teamOneSizeText.text = teamOneSize.ToString() + "/4";

        // Team Two
        foreach (Transform child in teamTwoPlayerDataHodler) {
            if (child == playerDataSingle) continue;
            Destroy(child.gameObject);
        }

        teamTwoSizeText.text = teamTwoSize.ToString() + "/4";
    }

    [ClientRpc]
    private void PopulateTeamOnePlayerDataHolderClientRpc(FixedString64Bytes playerName) {
        Transform playerDataTransform = Instantiate(playerDataSingle, teamOnePlayerDataHodler);
        PlayerDataSingleUI playerDataSingleUI = playerDataTransform.gameObject.GetComponent<PlayerDataSingleUI>();

        playerDataSingleUI.UpdatePlayerName(playerName.ToString());
        playerDataTransform.gameObject.SetActive(true);
    }
    
    [ClientRpc]
    private void PopulateTeamTwoPlayerDataHolderClientRpc(FixedString64Bytes playerName) {
        Transform playerDataTransform = Instantiate(playerDataSingle, teamTwoPlayerDataHodler);
        PlayerDataSingleUI playerDataSingleUI = playerDataTransform.gameObject.GetComponent<PlayerDataSingleUI>();

        playerDataSingleUI.UpdatePlayerName(playerName.ToString());
        playerDataTransform.gameObject.SetActive(true);
    }

    private void LobbyManager_OnLobbyUpdated(object sender, System.EventArgs e) {
        lobbyCode.text = LobbyManager.Instance.GetLobby().LobbyCode;
        if (!LobbyManager.Instance.IsLobbyHost()) {
            startButton.interactable = false;
        }
    }

    private void Awake() {
        backButton.onClick.AddListener(() => {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerData();
            LeaveGameServerRpc(playerData);

            LobbyManager.Instance.LeaveLobby();
            createJoinUI.Show();
            Hide();
        });

        startButton.onClick.AddListener(() => {
            SetTeamNamesClientRpc();
            SceneLoader.LoadSceneNetwork(SceneLoader.Scene.Game);
        });

        joinTeamOneButton.onClick.AddListener(() => {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerData();
            JoinTeamServerRpc(playerData, 1);
        });
        
        joinTeamTwoButton.onClick.AddListener(() => {
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerData();
            JoinTeamServerRpc(playerData, 2);
        });
    }

    private void Update() {
        // Check if all players are in teams
        // Check if each team has at least two players
        // ^- Team two can have 0 players
        if (LobbyManager.Instance.IsLobbyHost()) {
            int teamOneCount = MultiplayerManager.Instance.GetTeamOnePlayerDataList().Count;
            int teamTwoCount = MultiplayerManager.Instance.GetTeamTwoPlayerDataList().Count;
            int totalPlayerCount = MultiplayerManager.Instance.GetPlayerDataList().Count;

            if (teamOneCount + teamTwoCount != totalPlayerCount || teamTwoCount == 1 || teamOneCount < 2) {
                startButton.interactable = false;
            } else {
                startButton.interactable = true;
            }
        }
    }

    [ClientRpc]
    private void SetTeamNamesClientRpc() {
        MultiplayerManager.Instance.SetTeamNames(TeamOneNameInputField.text, TeamTwoNameInputField.text);
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveGameServerRpc(PlayerData playerData) {
        MultiplayerManager.Instance.LeaveGame(playerData);
    }

    [ServerRpc(RequireOwnership = false)]
    private void JoinTeamServerRpc(PlayerData playerData, int team) {
        MultiplayerManager.Instance.JoinTeam(playerData, team);
    }

    public override void OnDestroy() {
        base.OnDestroy();

        LobbyManager.Instance.OnLobbyUpdated -= LobbyManager_OnLobbyUpdated;
        MultiplayerManager.Instance.OnTeamJoined -= MultiplayerManager_OnTeamJoined;
    }
}
