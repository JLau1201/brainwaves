using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateJoinUI : BaseUI
{

    [Header("UIs")]
    [SerializeField] private TitleScreenUI titleScreenUI;
    [SerializeField] private LobbyUI lobbyUI;

    [Header("Buttons")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private TMP_InputField lobbyCodeInputField;

    private void Awake() {
        Hide();

        backButton.onClick.AddListener(() => {
            titleScreenUI.Show();
            Hide();
        });
        
        hostButton.onClick.AddListener(() => {
            string playerName = playerNameInputField.text;

            LobbyManager.Instance.CreateLobby(playerName);
        });
        
        joinButton.onClick.AddListener(() => {
            string playerName = playerNameInputField.text;
            string lobbyCode = lobbyCodeInputField.text;

            LobbyManager.Instance.JoinWithCode(playerName, lobbyCode);
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
}
