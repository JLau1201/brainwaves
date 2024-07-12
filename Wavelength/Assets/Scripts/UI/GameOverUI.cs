using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : BaseUI
{
    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button quitButton;

    private void Awake() {
        Hide();

        playAgainButton.onClick.AddListener(() => {
            SceneLoader.LoadSceneNetwork(SceneLoader.Scene.Game);
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });

        if (!NetworkManager.Singleton.IsHost) {
            playAgainButton.interactable = false;
        }
    }
}
