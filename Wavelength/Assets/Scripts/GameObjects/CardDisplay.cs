using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI rightCardText;
    [SerializeField] private TextMeshProUGUI leftCardText;

    [Header("CardData")]
    [SerializeField] private TextAsset cardData;


    private List<string[]> allCardsList = new List<string[]>();

    private void Start() {
        ParseCSV();
        ChooseRandomCard();
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

    private void ChooseRandomCard() {
        // Get random index
        int randomIndex = Random.Range(0, allCardsList.Count-1);

        // Set card text
        rightCardText.text = allCardsList[randomIndex][0];
        leftCardText.text = allCardsList[randomIndex][1];
    }
}
