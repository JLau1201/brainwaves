using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{

    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnConnectionApproved;
    public event EventHandler OnTeamJoined;

    private const int MAX_PLAYER_COUNT = 8;
    private const int MAX_TEAM_SIZE = 4;

    private List<PlayerData> playerDataList = new List<PlayerData>();

    private List<PlayerData> teamOneList = new List<PlayerData>();
    private List<PlayerData> teamTwoList = new List<PlayerData>();

    private PlayerData playerData;
    private FixedString64Bytes playerName;

    private string teamOneName;
    private string teamTwoName;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // *** NETWORKMANAGER ***

    public void StartHost() {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;

        NetworkManager.Singleton.StartHost();
        
        playerData = new PlayerData {
            clientId = NetworkManager.Singleton.LocalClientId,
            playerName = playerName,
        };

        playerDataList.Add(playerData);
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse) {
        if(SceneManager.GetActiveScene().name != SceneLoader.Scene.MainMenu.ToString()) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started!";
            return;
        }

        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_COUNT) {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Max numbers of players reached!";
        }
        
        connectionApprovalResponse.Approved = true;
        OnConnectionApproved?.Invoke(this, EventArgs.Empty);
    }

    public void StartClient() {
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;

        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        playerData = new PlayerData {
            clientId = clientId,
            playerName = playerName,
        };

        AddPlayerDataServerRpc(playerData);

        OnConnectionApproved?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerDataServerRpc(PlayerData playerData) {
        playerDataList.Add(playerData);
        OnTeamJoined?.Invoke(this, EventArgs.Empty);
    }

    public void Shutdown() {
        playerDataList.Clear();
        NetworkManager.Singleton.Shutdown();
    }

    // *** GAME MANAGER ***
    public int GetMaxPlayerCount() {
        return MAX_PLAYER_COUNT;
    }

    public void SetPlayerName(string playerName) {
        this.playerName = (FixedString64Bytes)playerName;
    }

    public void JoinTeam(PlayerData playerData, int team) {
        switch (team) {
            case 1:
                // Check if team is full
                if(teamOneList.Count == MAX_TEAM_SIZE) return;

                // Check if player is not already in that team
                if (!teamOneList.Contains(playerData)) {
                    teamOneList.Add(playerData);

                    // Check if player is in other team
                    if (teamTwoList.Contains(playerData)) {
                        teamTwoList.Remove(playerData);
                    }

                    OnTeamJoined?.Invoke(this, EventArgs.Empty);
                }

                break;
            case 2:
                // Check if team is full
                if (teamTwoList.Count == MAX_TEAM_SIZE) return;

                // Check if player is not already in that team
                if (!teamTwoList.Contains(playerData)) {
                    teamTwoList.Add(playerData);

                    // Check if player is in other team
                    if (teamOneList.Contains(playerData)) {
                        teamOneList.Remove(playerData);
                    }

                    OnTeamJoined?.Invoke(this, EventArgs.Empty);
                }

                break;
            default:
                break;
        }
    }

    public PlayerData GetPlayerData() {
        return playerData;
    }

    public List<PlayerData> GetTeamOnePlayerDataList() {
        return teamOneList;
    }

    public List<PlayerData> GetTeamTwoPlayerDataList() {
        return teamTwoList;
    }

    public List<PlayerData> GetPlayerDataList() {
        return playerDataList;
    }

    public void LeaveGame(PlayerData playerData) {
        playerDataList.Remove(playerData);
        if (teamOneList.Contains(playerData)) {
            teamOneList.Remove(playerData);
        } else {
            teamTwoList.Remove(playerData);
        }
    }

    public void SetTeamNames(string teamOneName, string teamTwoName) {
        this.teamOneName = teamOneName;
        this.teamTwoName = teamTwoName;
    }

    public string GetTeamOneName() {
        return teamOneName;
    }

    public string GetTeamTwoName() {
        return teamTwoName;
    }
}
