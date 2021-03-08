using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public CanvasGroup MainMenuCanvasGroup;
    public CanvasGroup SettingsPanelCanvasGroup;
    public SpriteRenderer BGImageSpriteRenderer;
    public Sprite DarkBGImage;
    public Sprite LightBGImage;
    public AudioSource audioSource;
    public Toggle SoundToggle;
    public static UIController instance;
    private void Start()
    {
        SettingsPanelCanvasGroup.alpha = 0;
        SettingsPanelCanvasGroup.interactable = false;
        SettingsPanelCanvasGroup.blocksRaycasts = false;

        if (PlayerPrefs.GetFloat("Volume") == 0)
        {
            audioSource.volume = 0;
            SoundToggle.isOn = false;
        }
        else
        {
            audioSource.volume = 1;
            SoundToggle.isOn = true;
        }
    }
    //Close Menu Ui and Begin Game
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
        Debug.Log("1"+ToggleValue);

        ToggleValue = !ToggleValue;

        Debug.Log("2"+ToggleValue);
        if (ToggleValue)
        {
            audioSource.volume = 0;
            PlayerPrefs.SetFloat("Volume", 0);
        }
        else
        {
            audioSource.volume = 1;
            PlayerPrefs.SetFloat("Volume", 1);
        }
    }

    
}
