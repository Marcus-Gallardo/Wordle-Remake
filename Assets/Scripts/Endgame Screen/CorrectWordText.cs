using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CorrectWordText : MonoBehaviour
{
    [SerializeField]
    private WordleGame game;

    private void Start()
    {
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();

        // Display the correct word
        tmp.text = game.correctWord;

        // Make the text color the same as the green from the inputSquares;
        tmp.color = game.greenColor;
    }
}
