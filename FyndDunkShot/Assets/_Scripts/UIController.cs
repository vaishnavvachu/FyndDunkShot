using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region PUBLIC VARIABLES
    [Header("Canvas Groups")]
    public CanvasGroup MainMenuCanvasGroup;
    public CanvasGroup SettingsPanelCanvasGroup;

    [Header("Audio Source")]
    public AudioSource _AudioSource;

    [Header("Other UI Elements")]
    public Toggle SoundToggle;
    public SpriteRenderer BGImageSpriteRenderer;

    [NonSerialized] public static UIController instance;

    #endregion


    private void Start()
    {
        //Hide Settings Panel Initially
        SettingsPanelCanvasGroup.alpha = 0;
        SettingsPanelCanvasGroup.interactable = false;
        SettingsPanelCanvasGroup.blocksRaycasts = false;

        //Check if the sound was Off
        if (PlayerPrefs.GetFloat("Volume") == 0)
        {
            //Keep it off
            _AudioSource.volume = 0;
            //Show Toggle Off
            SoundToggle.isOn = false;
        }
        else
        {
            //Keep it On
            _AudioSource.volume = 1;

            //Show Toggle On
            SoundToggle.isOn = true;
        }
    }

    //Close Main Menu and Begin Game
    public void BeginGame()
    {
        MainMenuCanvasGroup.alpha = 0;
        MainMenuCanvasGroup.interactable = false;
        MainMenuCanvasGroup.blocksRaycasts = false;
    }

    //Open Settings Panel
    public void OpenSettingsPanel()
    {
        SettingsPanelCanvasGroup.alpha = 1;
        SettingsPanelCanvasGroup.interactable = true;
        SettingsPanelCanvasGroup.blocksRaycasts = true;
    }

    //Close Settings Panel
    public void CloseSettingsPanel()
    {
        SettingsPanelCanvasGroup.alpha = 0;
        SettingsPanelCanvasGroup.interactable = false;
        SettingsPanelCanvasGroup.blocksRaycasts = false;
    }

    //Toggle Change BG Image
    public void ToggleNightMode(bool ToggleValue)
    {
        ToggleValue = !ToggleValue;

        if (ToggleValue)
        {
            BGImageSpriteRenderer.color = new Color32(48, 48, 48, 255);
        }
        else
        {
            BGImageSpriteRenderer.color = new Color32(255, 255, 255, 255);
        }
    }

    //Toggle Change Sound
    public void ToggleSound(bool ToggleValue)
    {

        ToggleValue = !ToggleValue;

        if (ToggleValue)
        {
            _AudioSource.volume = 0;

            //Store the Value in PlayerPrefs
            PlayerPrefs.SetFloat("Volume", 0);
        }
        else
        {
            _AudioSource.volume = 1;
            PlayerPrefs.SetFloat("Volume", 1);
        }
    }

    
}
