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

    private Animator animator;

    private void Awake() {
        Hide();
        animator = GetComponent<Animator>();
        animator.enabled = false;
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

    public override void Show() {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        animator.enabled = true;
    }
}
