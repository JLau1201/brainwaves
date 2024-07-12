using System;
using TMPro;
using UnityEngine;

public class AddScoreAnimation : MonoBehaviour
{
    public event EventHandler OnAddScoreAnimationFinished;

    private TextMeshProUGUI newScoreText;
    private Animator animator;
    private AnimatorStateInfo animatorStateInfo;
    private int loopCount = 1;

    private void Awake() {
        animator = GetComponent<Animator>();
        newScoreText = GetComponent<TextMeshProUGUI>();
        animator.enabled = false;
    }

    private void Update() {
        if (animator.enabled) {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if(animatorStateInfo.normalizedTime >= loopCount) {
                animator.enabled = false;
                newScoreText.fontSize = 0;
                loopCount++;

                OnAddScoreAnimationFinished?.Invoke(this, EventArgs.Empty);
            }
        }   
    }

    public void SetText(int newScore) {
        newScoreText.text = newScore.ToString();
    }

    public void PlayAnimation() {
        animator.enabled = true;
    }
}
