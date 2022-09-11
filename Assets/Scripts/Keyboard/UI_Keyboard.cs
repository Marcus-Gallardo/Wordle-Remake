using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI_Keyboard : MonoBehaviour
{
    [SerializeField]
    GameObject firstRow;

    [SerializeField]
    GameObject secondRow;

    [SerializeField]
    GameObject thirdRow;

    [SerializeField]
    GameObject keyPrefab;

    [SerializeField]
    WordleGame game;

    // Make lists to hold the keys for each row
    List<string> firstRowKeys = new List<string>();
    List<string> secondRowKeys = new List<string>();
    List<string> thirdRowKeys = new List<string>();

    [SerializeField]
    Sprite deleteKeySprite;

    // Create references to GameObjects of the keyboard and associate them with their assigned letter
    Dictionary<string, GameObject> keyboardKeys = new Dictionary<string, GameObject>();

    private void Awake()
    {
        // Add all the keys to the appropriate rows
        AddKeysToList();

        // Create all the key objects
        CreateKeyboardKeys();
    }

    /// <summary>
    /// Adds all of the keyboard keys to lists storing all the keys in each row of the keyboard
    /// </summary>
    private void AddKeysToList()
    {
        // Add the first row of keys to the firstRowKeys list
        firstRowKeys.Add("Q");
        firstRowKeys.Add("W");
        firstRowKeys.Add("E");
        firstRowKeys.Add("R");
        firstRowKeys.Add("T");
        firstRowKeys.Add("Y");
        firstRowKeys.Add("U");
        firstRowKeys.Add("I");
        firstRowKeys.Add("O");
        firstRowKeys.Add("P");

        // Add the second row of keys to the secondRowKeys list
        secondRowKeys.Add("A");
        secondRowKeys.Add("S");
        secondRowKeys.Add("D");
        secondRowKeys.Add("F");
        secondRowKeys.Add("G");
        secondRowKeys.Add("H");
        secondRowKeys.Add("J");
        secondRowKeys.Add("K");
        secondRowKeys.Add("L");

        // Add the third row of keys to the thirdRowKeys list
        thirdRowKeys.Add("ENTER");
        thirdRowKeys.Add("Z");
        thirdRowKeys.Add("X");
        thirdRowKeys.Add("C");
        thirdRowKeys.Add("V");
        thirdRowKeys.Add("B");
        thirdRowKeys.Add("N");
        thirdRowKeys.Add("M");
        thirdRowKeys.Add("DELETE");
    }

    /// <summary>
    /// Instantiates all keyboard keys
    /// </summary>
    private void CreateKeyboardKeys()
    {
        // Create all the keys for the first row
        foreach(string key in firstRowKeys)
        {
            GameObject keyInstance = Instantiate(keyPrefab, firstRow.transform);

            keyInstance.name = key;
            SetKeyboardKeyText(keyInstance, key);
            AddButtonListener(keyInstance);

            keyboardKeys.Add(key, keyInstance);
        }

        // Create all the keys for the second row
        foreach(string key in secondRowKeys)
        {
            GameObject keyInstance = Instantiate(keyPrefab, secondRow.transform);

            keyInstance.name = key;
            SetKeyboardKeyText(keyInstance, key);
            AddButtonListener(keyInstance);

            keyboardKeys.Add(key, keyInstance);
        }

        // Create all the keys for the third row
        foreach (string key in thirdRowKeys)
        {
            GameObject keyInstance = Instantiate(keyPrefab, thirdRow.transform);

            keyInstance.name = key;
            SetKeyboardKeyText(keyInstance, key);
            AddButtonListener(keyInstance);

            // We don't need to add the enter and delete keys to the keyboardKeys dictionary
            if(key != "ENTER" && key != "DELETE")
            {
                keyboardKeys.Add(key, keyInstance);
            }

            if(key == "ENTER")
            {
                AdjustEnterKey(keyInstance);
            }

            if(key == "DELETE")
            {
                AdjustDeleteKey(keyInstance);
            }
        }
    }

    /// <summary>
    /// Adds a listener to the button component of the child of the keyboard key
    /// </summary>
    /// <param name="keyInstance"></param>
    void AddButtonListener(GameObject keyInstance)
    {
        // Use a delegate to pass the HandleKeyboardInput() method to Addlistener() as a parameter
        keyInstance.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate { HandleKeyboardInput(keyInstance.name); });
    }

    /// <summary>
    /// Scales the children of the enter key to fit the keyboard properly
    /// </summary>
    /// <param name="keyInstance"></param>
    void AdjustEnterKey(GameObject keyInstance)
    {
        // Scale the child GameObject with the button and image components and adjust its position on the x axis
        keyInstance.transform.GetChild(0).localScale = new Vector3(1.586132f, 1f, 1f);

        // Adjust the button and image object's localPosition on the x axis
        Vector3 originalButtonPos = keyInstance.transform.GetChild(0).localPosition;
        keyInstance.transform.GetChild(0).localPosition = new Vector3(-47f, originalButtonPos.y, originalButtonPos.z);

        // Adjust the localPosition of the child GameObject that contains the text component to be centered according to the background
        Vector3 originalTextPos = keyInstance.transform.GetChild(1).localPosition;
        keyInstance.transform.GetChild(1).localPosition = new Vector3(-50f, originalTextPos.y, originalTextPos.z);

        // Change the size of the text so that it fits with the background better
        keyInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().fontSize = 20;
    }

    /// <summary>
    /// Scales the children of the delete key to fit the keyboard properly and adds the image for the delete key
    /// </summary>
    /// <param name="keyInstance"></param>
    void AdjustDeleteKey(GameObject keyInstance)
    {
        // Store a reference to the child with the button component
        GameObject buttonChild = keyInstance.transform.GetChild(0).gameObject;

        // Scale the child object with the button and image components and adjust its position on the x axis
        buttonChild.transform.localScale = new Vector3(1.586132f, 1f, 1f);

        // Adjust the localPosition on the x axis of the child object that contains the button component
        Vector3 originalButtonPos = buttonChild.transform.localPosition;
        buttonChild.transform.localPosition = new Vector3(47f, originalButtonPos.y, originalButtonPos.z);

        // Store a referecne to the child with a text component
        GameObject textChild = keyInstance.transform.GetChild(1).gameObject;

        // Adjust the localPosition of the child to center it on the background
        Vector3 originalTextPos = textChild.transform.localPosition;

        textChild.transform.localPosition = new Vector3(50f, originalTextPos.y, originalTextPos.z);
        // Destroy the child's text component
        DestroyImmediate(textChild.GetComponent<TextMeshProUGUI>()); // Destroy() is called at end of frame, but DestroyImmediate() is not

        // Add image component to the child
        Image image = textChild.AddComponent<Image>();

        // Set the newly added image component to the deleteKeyImage
        image.sprite = deleteKeySprite;

        // Prevent image component from being a raycast object
        image.raycastTarget = false;

        // Set the scale of the child
        textChild.transform.localScale = new Vector3(0.5200157f, 0.31834f, 0f);

        // Rename child
        textChild.name = "Image";
    }
    
    /// <summary>
    /// Sets the text of the text component of the child of the given keyboard key
    /// </summary>
    /// <param name="keyInstance"></param>
    /// <param name="key"></param>
    void SetKeyboardKeyText(GameObject keyInstance, string key)
    {
        keyInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = key;
    }

    /// <summary>
    /// Handles the actions for the different types of keyboard keys that can be pressed
    /// </summary>
    /// <param name="input"></param>
    public void HandleKeyboardInput(string input)
    {
        // Don't handle the input if we can't input letters
        if(!game.canInputLetters)
        {
            return;
        }

        switch(input)
        {
            case "ENTER":
                game.InitiateCheckGuess();
                return;

            case "DELETE":
                game.DeleteLetter();
                return;

            default:
                game.AddLetter(input);
                return;
        }
    }

    public void UpdateKeyboardColors()
    {
        int numTimesRun = 0;

        foreach(KeyValuePair<string, TileColor> kvp in game.letterInfo)
        {
            SetKeyboardKeyColor(keyboardKeys[kvp.Key], kvp.Value);

            numTimesRun++;
        }
    }

    void SetKeyboardKeyColor(GameObject key, TileColor color)
    {
        key.transform.GetChild(0).GetComponent<Image>().color = game.GetTileColor(color);
    }
}
