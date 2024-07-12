using System;
using TMPro;
using UnityEngine;

public class TransitionUI : MonoBehaviour
{

    public event EventHandler OnTransitionAnimationFinished;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI roleText;
    [SerializeField] private TextMeshProUGUI turnStartText;

    private Animator animator;
    private AnimatorStateInfo animatorStateInfo;
    private int loopCount = 1;

    private void Awake() {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    public void SetText(string role, string playerName) {
        if (role == "") {
            roleText.text = "Game Over";
            turnStartText.text = "Team " + playerName + " Wins!";
        } else if (playerName == "") {
            roleText.text = "Game Start";
            turnStartText.text = "Team 1 Goes First!";
        } else {
            roleText.text = role;
            turnStartText.text = playerName + " Turn Start!";
        }
    }

    public void PlayAnimation() {
        animator.enabled = true;
    }

    private void Update() {
        if (animator.enabled) {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if(animatorStateInfo.normalizedTime >= loopCount) {
                animator.enabled = false;
                loopCount++;
                OnTransitionAnimationFinished?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
