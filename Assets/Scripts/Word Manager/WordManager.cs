using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.IO;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    // This list holds the words that are valid guesses
    List<string> guessList = new List<string>();

    // This list holds the words that are not esoteric; they are guessable and therefore valid solutions to the Wordle
    List<string> solutionList = new List<string>();

    [SerializeField]
    TextAsset possibleGuesses;

    [SerializeField]
    TextAsset possibleSolutions;

    private void Awake()
    {
        //InitializeWordLists();

        guessList = ConvertWordStringToArr(possibleGuesses.text);
        solutionList = ConvertWordStringToArr(possibleSolutions.text);

        guessList = ConvertWordsToUpper(guessList);
        solutionList = ConvertWordsToUpper(solutionList);
    }

    List<string> ConvertWordStringToArr(string wordString)
    {
        // Store the given words
        var newWords = wordString;

        // Remove the left square bracket at the beginning of the string
        newWords = newWords.TrimStart('[');

        // Remove the right square bracket at the end of the string
        newWords = newWords.TrimEnd(']');

        // Remove all the quotation marks in the string
        newWords = newWords.Replace("\"", string.Empty);

        // Remove all spaces in the string
        newWords = newWords.Replace(" ", "");

        var wordArr = newWords.Split(',');

        List<string> wordList = new List<string>();

        // Convert wordArr into a list
        for(int i = 0; i < wordArr.Length; i++)
        {
            wordList.Add(wordArr[i]);
        }

        return wordList;
    }

    /// <summary>
    /// Returns a random word from the list of possible solutions
    /// </summary>
    /// <returns></returns>
    public string GetRandomSolution()
    {
        int randomIndex = Random.Range(0, solutionList.Count);

        string word = solutionList[randomIndex];

        print("Random word is " + word);

        return word;
    }

    /// <summary>
    /// Converts all words in the given word list to uppercase to make comparing them to other strings easier
    /// </summary>
    List<string> ConvertWordsToUpper(List<string> listToConvert)
    {
        for(int i = 0; i < listToConvert.Count; i++)
        {
            listToConvert[i] = listToConvert[i].ToUpper();
        }

        return listToConvert;
    }

    /// <summary>
    /// Checks if a given string is in the list of guesses
    /// </summary>
    /// <param name="wordToCheck"></param>
    /// <returns></returns>
    public bool CheckWord(string wordToCheck)
    {
        if(guessList.Contains(wordToCheck))
        {
            return true;
        }

        return false;
    }
}
