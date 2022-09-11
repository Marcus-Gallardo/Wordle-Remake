using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public enum FlipMode
{
    Original,
    Fast,
    Simultaneous
}

public class UI_SettingsManager : MonoBehaviour
{
    [SerializeField]
    WordleGame game;

    [SerializeField]
    WarningManager warningManager;

    public bool menuOpen = false;

    Animator animator;

    public bool hardModeEnabled = false;

    [SerializeField]
    GameObject hardModeSetting;

    [SerializeField]
    GameObject flipModeSetting;

    [SerializeField]
    GameObject menuButton;

    [SerializeField]
    GameObject exitButton;

    public FlipMode flipMode = FlipMode.Original;

    private void Start()
    {
        animator = GetComponent<Animator>();

        AddListenerToMenuButton();
        AddListenerToHardModeToggle();
        AddListenerToFlipModeDropdown();
        AddListenerToExitButton();
    }

    void AddListenerToMenuButton()
    {
        menuButton.GetComponent<Button>().onClick.AddListener(delegate { HandleMenuButtonClick(); });
    }

    public async Task OpenSettingsMenu()
    {
        // Prevent player from inputting letters while this menu is open
        game.canInputLetters = false;

        animator.SetTrigger("triggerOpen");

        menuOpen = true;

        await Task.Delay((int)GetAnimationClipLength(animator, "Open") * 1000);
    }

    public async Task CloseSettingsMenu()
    {
        // Allow player to input letters again
        game.canInputLetters = true;

        animator.SetTrigger("triggerClose");

        menuOpen = false;

        await Task.Delay((int)GetAnimationClipLength(animator, "Open") * 1000);
    }

    async void HandleMenuButtonClick()
    {
        if(menuOpen)
        {
            await CloseSettingsMenu();
        }
        else if(!menuOpen)
        {
            await OpenSettingsMenu();
        }
    }

    void AddListenerToHardModeToggle()
    {
        hardModeSetting.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate { ToggleHardMode(); });
    }

    async void ToggleHardMode()
    {
        // Don't allow the player to activate hard mode if a guess has been inputted
        if(game.currentRow > 0)
        {
            warningManager.InstantiateWarning("Hard mode can only be enabled at the start of a round");
            return;
        }

        if (hardModeEnabled == false)
        {
            hardModeEnabled = true;
        }
        else
        {
            hardModeEnabled = false;
        }

        await AnimateHardModeToggle(hardModeEnabled);
    }

    async Task AnimateHardModeToggle(bool toggle)
    {
        Animator animator = hardModeSetting.transform.GetChild(1).GetComponent<Animator>();

        if(toggle)
        {
            animator.SetTrigger("triggerTrue");
        }

        if(!toggle)
        {
            animator.SetTrigger("triggerFalse");
        }

        await Task.Delay((int)GetAnimationClipLength(animator, "ToggleTrue") * 1000);
    }

    /// <summary>
    /// Gets the length of an animation clip based on the given animator and clip name
    /// <para> Returns 0 if the clip is not found </para>
    /// </summary>
    /// <param name="animator"></param>
    /// <param name=""></param>
    /// <returns></returns>
    float GetAnimationClipLength(Animator animator, string clipName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0;
    }

    void AddListenerToFlipModeDropdown()
    {
        flipModeSetting.transform.GetChild(1).GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate { UpdateFlipMode(); });
    }

    void UpdateFlipMode()
    {
        switch(flipModeSetting.transform.GetChild(1).GetComponent<TMP_Dropdown>().value)
        {
            case 0:
                flipMode = FlipMode.Original;
                break;

            case 1:
                flipMode = FlipMode.Fast;
                break;

            case 2:
                flipMode = FlipMode.Simultaneous;
                break;
        }
    }

    void AddListenerToExitButton()
    {
        exitButton.GetComponent<Button>().onClick.AddListener(delegate { ExitGame(); });
    }

    void ExitGame()
    {
        if(Application.isEditor)
        {
            Debug.Break(); 
        }
        else
        {
            Application.Quit();
        }
    }
}
