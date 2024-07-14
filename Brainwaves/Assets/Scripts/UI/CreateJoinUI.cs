using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateJoinUI : BaseUI
{

    [Header("UIs")]
    [SerializeField] private TitleScreenUI titleScreenUI;
    [SerializeField] private ConnectingUI connectingUI;
    [SerializeField] private LobbyUI lobbyUI;

    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField lobbyCodeInputField;

    private void Start() {
        MultiplayerManager.Instance.OnConnectionApproved += MultiplayerManager_OnConnectionApproved;
        LobbyManager.Instance.OnLobbyJoinError += LobbyManager_OnLobbyJoinError;
    }

    private void LobbyManager_OnLobbyJoinError(object sender, System.EventArgs e) {
        connectingUI.Hide();
    }

    private void MultiplayerManager_OnConnectionApproved(object sender, System.EventArgs e) {
        Hide();
        connectingUI.Hide();
        lobbyUI.Show();
    }

    private void Awake() {
        Hide();

        int randInt = Random.Range(1, 100);
        playerNameInputField.text = "Player " + randInt.ToString();

        backButton.onClick.AddListener(() => {
            titleScreenUI.Show();
            Hide();
        });
        
        hostButton.onClick.AddListener(() => {
            string playerName = playerNameInputField.text;
            connectingUI.Show();
            LobbyManager.Instance.CreateLobby();
            MultiplayerManager.Instance.SetPlayerName(playerName);
        });
        
        joinButton.onClick.AddListener(() => {
            string playerName = playerNameInputField.text;
            string lobbyCode = lobbyCodeInputField.text;
            connectingUI.Show();
            LobbyManager.Instance.JoinWithCode(lobbyCode);
            MultiplayerManager.Instance.SetPlayerName(playerName);
        });

        // Check for valid lobby code
        lobbyCodeInputField.onValueChanged.AddListener(delegate {
            string lobbyCode = lobbyCodeInputField.text;
            
            if(lobbyCode.Length == 6) {
                joinButton.interactable = true;
            } else {
                joinButton.interactable = false;
            }
        });
    }

    public override void OnDestroy() {
        base.OnDestroy();
        MultiplayerManager.Instance.OnConnectionApproved -= MultiplayerManager_OnConnectionApproved;
        LobbyManager.Instance.OnLobbyJoinError -= LobbyManager_OnLobbyJoinError;
    }
}
