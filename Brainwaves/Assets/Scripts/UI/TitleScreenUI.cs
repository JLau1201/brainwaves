using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenUI : BaseUI
{

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    [Header("UIs")]
    [SerializeField] private CreateJoinUI createJoinUI;

    private void Awake() {
        playButton.onClick.AddListener(() => {
            createJoinUI.Show();
            Hide();
        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
