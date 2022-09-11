using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;

public enum TileColor
{
    Green,
    Yellow,
    Gray
}

public class WordleGame : MonoBehaviour
{
    public GameObject inputSquarePrefab;
    public GameObject UI_InputSquares;
    public GameObject UI_EndgameScreen;
    public GameObject UI_Keyboard;
    public GameObject UI_RestartGameButton;

    public WarningManager warningManager;
    public WordManager wordManager;
    public InputSquareManager inputSquareManager;
    public UI_SettingsManager settingsManager;

    public readonly int numRows = 6;
    public readonly int numCols = 5;

    public readonly int rowLength = 5;
    public readonly int colLength = 6;

    // Make the information about the colors of the inputSquares public
    public readonly Color greenColor = new Color(0.2705882f, 0.5843138f, 0.145098f, 1);
    public readonly Color yellowColor = new Color(0.7843138f, 0.6313726f, 0.06666667f, 1);
    public readonly Color grayColor = new Color(0.2784314f, 0.2941177f, 0.3019608f, 1);

    // These two colors are for the border of the inputSquare
    public readonly Color lightGrayBorderColor = new Color(0.3372549f, 0.3411765f, 0.345098f, 1);
    public readonly Color defaultBorderColor = new Color(0.227451f, 0.227451f, 0.2352941f, 1);

    private float spacing;
    readonly int inputSquareLength = 100;
    readonly int inputSquareWidth = 100;

    public int currentRow;
    public int currentColumn;

    public bool canInputLetters = true;

    GameObject[,] inputSquareArray;
   
    public string correctWord = "crane";

    List<string> possibleLettersList;

    List<string> winMessages = new List<string>();
    List<string> loseMessages = new List<string>();

    Stopwatch gameTimer;

    [SerializeField]
    Canvas mainCanvas;

    // Dictionary holding the information about each letter
    public Dictionary<string, TileColor> letterInfo = new Dictionary<string, TileColor>();

    private void Awake()
    {
        spacing = GetInputSquareSpacing();
        inputSquareArray = inputSquareManager.InstantiateInputSquares(numRows, numCols);

        currentRow = 0;
        currentColumn = 0;

        inputSquareManager.LayoutInputSquares(inputSquareManager.GetInputSquares(), rowLength, colLength, spacing, inputSquareLength, inputSquareWidth);

        // Hide the endgame screen
        UI_EndgameScreen.SetActive(false);

        gameTimer = new Stopwatch();

        InitializeMessageLists();
        InitializePossibleLetterList();
        InitializeGame();
        AddListenerToRestartGameButton();

        StartGameTimer(gameTimer);
    }

    private void OnGUI()
    {
        Event e = Event.current;

        if (e.keyCode.ToString() != "None")
        {
            if (e.isKey && e.type == EventType.KeyDown && canInputLetters)
            {
                string keycodeToString = e.keyCode.ToString();

                if (possibleLettersList.Contains(keycodeToString))
                {
                    AddLetter(e.keyCode.ToString());
                }
                else if (keycodeToString == "Backspace")
                {
                    DeleteLetter();
                }
                else if (keycodeToString == "Return")
                { 
                    // Check if any of the inputSquares in the current row do not have any letter in them
                    for(int i = 0; i < rowLength; i++)
                    {
                        GameObject inputSquare = inputSquareArray[currentRow, i];

                        if(inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text == string.Empty)
                        {
                            // Alert the player that the guess is incomplete
                            warningManager.InstantiateWarning("Not enough letters");
                            inputSquareManager.ShakeInputSquaresAsync(GetCurrentRowOfInputSquares());

                            return;
                        }
                    }

                    InitiateCheckGuess();
                }
            }
        }
    }

    GameObject GetCurrentInputSquare()
    {
        return inputSquareArray[currentRow, currentColumn];
    }

    GameObject[] GetCurrentRowOfInputSquares()
    {
        GameObject[] currentSquareRow = new GameObject[5];

        for(int i = 0; i < 5; i++)
        {
            currentSquareRow[i] = inputSquareArray[currentRow, i];
        }

        return currentSquareRow;
    }

    void InitializeGame()
    {
        wordManager = GameObject.FindGameObjectWithTag("WordManager").GetComponent<WordManager>();

        correctWord = wordManager.GetRandomSolution();
        correctWord = correctWord.ToUpper();
    }

    void InitializePossibleLetterList()
    {
        possibleLettersList = new List<string>();

        possibleLettersList.Add("A");
        possibleLettersList.Add("B");
        possibleLettersList.Add("C");
        possibleLettersList.Add("D");
        possibleLettersList.Add("E");
        possibleLettersList.Add("F");
        possibleLettersList.Add("G");
        possibleLettersList.Add("H");
        possibleLettersList.Add("I");
        possibleLettersList.Add("J");
        possibleLettersList.Add("K");
        possibleLettersList.Add("L");
        possibleLettersList.Add("M");
        possibleLettersList.Add("N");
        possibleLettersList.Add("O");
        possibleLettersList.Add("P");
        possibleLettersList.Add("Q");
        possibleLettersList.Add("R");
        possibleLettersList.Add("S");
        possibleLettersList.Add("T");
        possibleLettersList.Add("U");
        possibleLettersList.Add("V");
        possibleLettersList.Add("W");
        possibleLettersList.Add("X");
        possibleLettersList.Add("Y");
        possibleLettersList.Add("Z");
    }

    public void AddLetter(string letter)
    {
        GameObject inputSquare = GetCurrentInputSquare();

        // There are only 5 letters to a word in Wordle, and arrays are 0 based 
        if(currentColumn == 4)
        {
            if (inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text != string.Empty)
            {
                return;
            }

            inputSquareManager.SetInputSquareBorderColor(inputSquare, lightGrayBorderColor);
            StartCoroutine(inputSquareManager.AnimateInputSquarePop(inputSquare));

            inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = letter;
            return;
        }

        inputSquareManager.SetInputSquareBorderColor(inputSquare, lightGrayBorderColor);
        StartCoroutine(inputSquareManager.AnimateInputSquarePop(inputSquare));

        inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = letter;

        // Change the current position to the next box
        currentColumn++;
    }

    public void DeleteLetter()
    {
        GameObject inputSquare = GetCurrentInputSquare();

        // You cannot have a negative amount of letters in your word, and arrays are 0 based
        if (currentColumn == 0)
        {
            inputSquareManager.SetInputSquareBorderColor(inputSquare, defaultBorderColor);
            inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = string.Empty;
            return;
        }

        if(inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text == string.Empty)
        {
            currentColumn--;
            inputSquareManager.SetInputSquareBorderColor(GetCurrentInputSquare(), defaultBorderColor);
            GetCurrentInputSquare().transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = string.Empty;
        }
        else
        {
            inputSquareManager.SetInputSquareBorderColor(inputSquare, defaultBorderColor);
            inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = string.Empty;
        }
    }

    public async void InitiateCheckGuess()
    {
        // The player should not be able to check their guesses if input is disabled!
        if(!canInputLetters)
        {
            return;
        }

        // The player should not be able to input during the checking of letters
        canInputLetters = false;

        string guess = string.Empty;

        // Loop through the current row of inputSquares and collect their letters to form the guess
        for (int i = 0; i < 5; i++)
        {
            GameObject inputSquare = inputSquareArray[currentRow, i];

            // If the text is empty, that means the guess is incomplete
            if (inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text == string.Empty)
            {
                break;
            }

            guess += inputSquare.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text;
        }

        // Check if the guess is an actual word
        if(!wordManager.CheckWord(guess))
        {
            // Animate the current row of input squares
            inputSquareManager.ShakeInputSquaresAsync(GetCurrentRowOfInputSquares());

            // Create a warning to tell the player that the guess is not a word
            warningManager.InstantiateWarning("Not in word list");

            // Checking failed, but the player still needs to input letters!
            canInputLetters = true;

            return;
        }

        // Check if hard mode is enabled
        if(settingsManager.hardModeEnabled)
        { 
            foreach(KeyValuePair<string, TileColor> kvp in letterInfo)
            {
               // Render the guess invalid if it does not contain every letter that is either green or yellow
               if(kvp.Value == TileColor.Yellow  || kvp.Value == TileColor.Green)
               {
                    if(!guess.Contains(kvp.Key))
                    {
                        // Alert the player through a warning
                        warningManager.InstantiateWarning($"Guess must contain {kvp.Key}");

                        // Allow the player to input letters again
                        canInputLetters = true;
                        return;
                    }
               }
            }
        }

        TileColor[] colors = CheckGuess(correctWord, guess);

        this.UI_Keyboard.GetComponent<UI_Keyboard>().UpdateKeyboardColors();

        await SetInputSquareColors(colors);

        // If there are no yellow or gray tiles in the guess, the player won the game
        if (!colors.Contains(TileColor.Yellow) && !colors.Contains(TileColor.Gray))
        {
            EndGame(true);
        }

        // If the player failed on the last guess, end the game
        if(colors.Contains(TileColor.Yellow) || colors.Contains(TileColor.Gray))
        {
            if(currentRow + 1 == numRows)
            {
                EndGame(false);
            }
        }

        MoveToNextRow();

        // After checking has been completed, allow the player to input letters again
        canInputLetters = true;
    }

    void MoveToNextRow()
    {
        currentRow++;
        currentColumn = 0;
    }

    void InitializeMessageLists()
    {
        winMessages.Add("Nice job!");
        winMessages.Add("You did it!");
        winMessages.Add("Not so hard, huh?");
        winMessages.Add("Maybe try hard mode?");
        winMessages.Add("Are you cheating?");

        loseMessages.Add("Good try!");
        loseMessages.Add("You'll get it next time!");
        loseMessages.Add("Hmmm, tricky?");
        loseMessages.Add("You win some, you lose some.");
        loseMessages.Add("Not so easy is it now?");
    }

    void StartGameTimer(Stopwatch timer)
    {
        timer.Reset();
        timer.Start();
    }

    void StopGameTimer(Stopwatch timer)
    {
        timer.Stop();
    }

    TileColor[] CheckGuess(string correctWord, string guess)
    {
        // Initialize the array of the colors for the guess
        // This list should be length 5 because every Wordle guess has 5 letters
        TileColor[] resultColors = new TileColor[5];

        // Initialize array of the correct letters in the correctWord
        string[] correctLetters = new string[5];

        // Add all 5 letters of the correctWord to the correctLetters array
        for(int i = 0; i < 5; i++)
        {
            correctLetters[i] = correctWord[i].ToString();
        }

        // Set all colors to gray at beginning
        // If a tile does not get set, then it should be gray
        for (int i = 0; i < 5; i++)
        {
            resultColors[i] = TileColor.Gray;
        }

        // Check for all greens in the guess
        // This loop should run only five times because every guess must have exactly 5 letters 
        for (int i = 0; i < 5; i++)
        {
            string currentLetter = guess[i].ToString();

            if(correctLetters[i] == currentLetter)
            {
                resultColors[i] = TileColor.Green;

                correctLetters[i] = "_";

                // Replace all information about the current letter
                if(letterInfo.ContainsKey(currentLetter))
                {
                    letterInfo.Remove(currentLetter);
                }

                letterInfo.Add(currentLetter, TileColor.Green);
            }
        }

        // Loop through all 5 letters in guess string
        for(int i = 0; i < 5; i++)
        {
            string currentLetter = guess[i].ToString();

            // Check for grays
            if(!correctLetters.Contains(currentLetter) && correctLetters[i] != "_")
            {         
                resultColors[i] = TileColor.Gray;

                if (letterInfo.ContainsKey(currentLetter))
                {
                    if (letterInfo[currentLetter] != TileColor.Green)
                    {
                        letterInfo.Remove(currentLetter);
                        letterInfo.Add(currentLetter, TileColor.Gray);
                    }
                }
                else
                {
                    letterInfo.Add(currentLetter, TileColor.Gray);
                }
            }
            
            // Check for yellows
            if(correctLetters.Contains(currentLetter) && currentLetter != "_")
            {
                if(correctLetters[i] != currentLetter)
                {
                    if (resultColors[i] != TileColor.Green)
                    {
                        int index = Array.IndexOf(correctLetters, currentLetter);

                        correctLetters[index] = "_";

                        resultColors[i] = TileColor.Yellow;

                        if(letterInfo.ContainsKey(currentLetter))
                        {
                            if(letterInfo[currentLetter] != TileColor.Green)
                            {
                                letterInfo.Remove(currentLetter);
                                letterInfo.Add(currentLetter, TileColor.Yellow);
                            }
                        }
                        else
                        {
                            letterInfo.Add(currentLetter, TileColor.Yellow);
                        }
                    }        
                }
            }
        }

        return resultColors;
    }

    /// <summary>
    /// Gets the current row of inputSquares and sets them to the given colors
    /// </summary>
    /// <param name="colorsToUse"></param>
    async Task SetInputSquareColors(TileColor[] colorsToUse)
    {
        // Store current row of inputSquares
        GameObject[] row = new GameObject[5];

        // Get current row of inputSquares
        for(int i = 0; i < row.Length; i++)
        {
            row[i] = inputSquareArray[currentRow, i];
        }

        switch(settingsManager.flipMode)
        {
            case FlipMode.Simultaneous:
                for(int i = 0; i < row.Length; i++)
                {
#pragma warning disable CS4014
                    inputSquareManager.ShrinkInputSquareAsync(row[i]);
#pragma warning restore CS4014
                }

                await Task.Delay(300);

                for (int j = 0; j < row.Length; j++)
                {
                    switch (colorsToUse[j])
                    {
                        // Make SetColor() a coroutine to ensure that the colors are set after the input square flips (for simultaneous flip mode)??
                        case TileColor.Green:
                            row[j].GetComponent<InputSquare>().SetColor(greenColor);
                            break;

                        case TileColor.Yellow:
                            row[j].GetComponent<InputSquare>().SetColor(yellowColor);
                            break;

                        case TileColor.Gray:
                            row[j].GetComponent<InputSquare>().SetColor(grayColor);
                            break;
                    }
                }

                for(int k = 0; k < row.Length; k++)
                {
#pragma warning disable CS4014
                    inputSquareManager.ExpandInputSquareAsync(row[k]);
#pragma warning restore CS4014
                }
                break;

            default:
                // For each of the squares, call InputSquare.SetColor() based on the color in colorsToUse
                for (int i = 0; i < row.Length; i++)
                {
                    // Wait for the inputSquare to shrunk
                    await inputSquareManager.ShrinkInputSquareAsync(row[i]);

                    switch (colorsToUse[i])
                    {
                        // Make SetColor() a coroutine to ensure that the colors are set after the input square flips (for simultaneous flip mode)??
                        case TileColor.Green:
                            row[i].GetComponent<InputSquare>().SetColor(greenColor);
                            break;

                        case TileColor.Yellow:
                            row[i].GetComponent<InputSquare>().SetColor(yellowColor);
                            break;

                        case TileColor.Gray:
                            row[i].GetComponent<InputSquare>().SetColor(grayColor);
                            break;
                    }

                    switch (settingsManager.flipMode)
                    {
                        case FlipMode.Original:
                            // Wait for the inputSquare to be expanded
                            await inputSquareManager.ExpandInputSquareAsync(row[i]);

                            // Wait in between the flipping of two squares
                            await Task.Delay(100);
                            break;

                        case FlipMode.Fast:
#pragma warning disable CS4014
                            inputSquareManager.ExpandInputSquareAsync(row[i]);
#pragma warning restore CS4014

                            // Wait in between the flipping of two squares
                            await Task.Delay(150);
                            break;

                        case FlipMode.Simultaneous:
#pragma warning disable CS4014
                            inputSquareManager.ExpandInputSquareAsync(row[i]);
#pragma warning restore CS4014
                            break;

                    }
                }
                break;
        }
    }

    async void EndGame(bool wonGame)
    {
        // Stop the game timer
        StopGameTimer(gameTimer);

        // The screen is not active by default so that it does not show during the game, but now we should make it show.
        UI_EndgameScreen.SetActive(true);

        // The player should not be able to play the game after the Wordle is lost or won
        canInputLetters = false;

        string message = string.Empty;

        // Set a message based on whether the player won or lost
        if(wonGame)
        {
            int random = UnityEngine.Random.Range(0, winMessages.Count);

            message = winMessages[random];
        }
        else
        {
            int random = UnityEngine.Random.Range(0, loseMessages.Count);

            message = loseMessages[random];
        }

        UI_EndgameScreen.SetActive(true);

        // Get the child object that holds the text component for displaying elapsed time
        GameObject elapsedTimeObject = UI_EndgameScreen.transform.GetChild(4).gameObject;

        // Get the TimeSpan from the gameTimer
        TimeSpan ts = gameTimer.Elapsed;

        string format = string.Empty;

        if (ts.Minutes < 10 && ts.Seconds < 10)
        {
            format = "0{0}:0{1}:{2}";
        }
        else if (ts.Minutes < 10)
        {
            format = "0{0}:{1}:{2}";
        }
        else if(ts.Seconds < 10)
        {
            format = "{0}:0{1}:{2}";
        }
        else
        {
            format = "{0}:{1}:{2}";
        }

        // Use the TimeSpan to display the total time on the text component of the child object
        elapsedTimeObject.GetComponent<TextMeshProUGUI>().text = String.Format(format, ts.Minutes, ts.Seconds, ts.Milliseconds);

        // Get the child object that holds the endgame message
        GameObject messageObject = UI_EndgameScreen.transform.GetChild(1).gameObject;

        // Set the text of the text component to the message
        messageObject.GetComponent<TextMeshProUGUI>().text = message;

        // Start animation on Win Screen
        Animator animator = UI_EndgameScreen.GetComponent<Animator>();

        animator.SetTrigger("triggerAppear");

        // Wait a little so that the animation can finish playing
        await Task.Delay(1);

        // Run this after triggering the appearence of the win screen so that the children are active
        // This allows the Animator component of newWordObject to be found
    }

    /// <summary>
    /// Reloads the current scene (which should be the game scene!)
    /// </summary>
    // This method had 0 references but is called when the "Play Again" button is pressed on the UI_EndgameScreen.
    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(scene.name);
    }

    /// <summary>
    /// Returns an RGBA color based on the given TileColor. A color of (0, 0, 0, 0) will be returned if the TileColor is invalid.
    /// </summary>
    /// <returns></returns>
    public Color GetTileColor(TileColor color)
    {
        switch(color)
        {
            case TileColor.Green:
                return greenColor;

            case TileColor.Yellow:
                return yellowColor;

            case TileColor.Gray:
                return grayColor;
        }

        return new Color(0, 0, 0, 0);
    }

    void AddListenerToRestartGameButton()
    {
        UI_RestartGameButton.GetComponent<Button>().onClick.AddListener(delegate { EndGame(false); });
        UI_RestartGameButton.GetComponent<Button>().onClick.AddListener(delegate { Test(); });
    }

    void Test()
    {
        print("Button pressed");
    }

    private float GetInputSquareSpacing()
    {
        switch(Screen.width)
        {
            case 1980:
                return 105;

            default:
                return 110;       
        }
    }
}
