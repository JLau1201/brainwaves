using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CardDisplay : NetworkBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI rightCardText;
    [SerializeField] private TextMeshProUGUI leftCardText;

    [Header("CardData")]
    [SerializeField] private TextAsset cardData;

    private Animator animator;
    private AnimatorStateInfo animatorStateInfo;
    private int currLoopCount = 15;
    private int loopAmount = 15;


    private List<string[]> allCardsList = new List<string[]>();

    private void Awake() {
        ParseCSV();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        ResetCards();
    }

    private void ParseCSV() {
        // Split csv by new lines
        string[] lines = cardData.text.Split(new string[] { "\n" }, System.StringSplitOptions.None);

        // Store each line as array of 2 cards
        foreach(string line in lines) {
            string[] cards = line.Split(',');
            allCardsList.Add(cards);
        }
    }

    private void ResetCards() {
        rightCardText.text = "";
        leftCardText.text = "";
    }

    public void ChooseRandomCard() {
        // Get random index
        int randomIndex = UnityEngine.Random.Range(0, allCardsList.Count - 1);
        string rightWord = allCardsList[randomIndex][1];
        string leftWord = allCardsList[randomIndex][0];

        PlayCardSelectionAnimationClientRpc(rightWord, leftWord);
    }

    [ClientRpc]
    private void PlayCardSelectionAnimationClientRpc(string rightWord, string leftWord) {
        rightCardText.text = rightWord;
        leftCardText.text = leftWord;
        animator.enabled = true;
    }

    private void Update() {
        if (animator.enabled) {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.normalizedTime >= currLoopCount) {
                animator.enabled = false;
                currLoopCount += loopAmount;
            }
        }
    }
}
