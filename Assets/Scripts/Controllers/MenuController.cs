using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Menu Components")]
    public GameObject mainPanel;
    public GameObject readmePanel;
    public GameObject settingsPanel;
    public GameObject settingsVideoPanel;
    public GameObject settingsAudioPanel;

    [Header("Settings Menu Components")]
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropDown;
    public Toggle fullScreenToggle;

    Resolution[] resolutions;
    List<string> qualitySettings;

    //PlayerPrefs Keys
    string playerPrefQualityKey = "graphicsQuality";
    string playerPrefResolutionKey = "resolution";

    //Global Graphics Quality Value
    int selectedQualityIndex;
    int selectedResolutionIndex;

    private void Awake()
    {
        qualitySettings = new List<string>();

        selectedQualityIndex = PlayerPrefs.GetInt(playerPrefQualityKey);
        selectedResolutionIndex = PlayerPrefs.GetInt(playerPrefResolutionKey);

        GetFullScreen();
        GetResolutions();
        GetQualitySettings();
    }

    public void NewGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ShowMainMenu()
    {
        settingsPanel.SetActive(false);
        readmePanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ShowReadmeMenu()
    {
        mainPanel.SetActive(false);
        readmePanel.SetActive(true);
    }

    public void ShowSettingsMenu()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void ShowSettingsVideo()
    {
        settingsAudioPanel.SetActive(false);
        settingsVideoPanel.SetActive(true);
    }

    public void ShowSettingsAudio()
    {
        settingsVideoPanel.SetActive(false);
        settingsAudioPanel.SetActive(true);        
    }    

    public void QuitGame()
    {
        Application.Quit();
    }

    #region Settings

    private void GetQualitySettings()
    {
        qualityDropDown.ClearOptions();

        foreach (string item in QualitySettings.names)
        {
            qualitySettings.Add(item);
        }

        qualityDropDown.AddOptions(qualitySettings);
        qualityDropDown.value = selectedQualityIndex;
        qualityDropDown.RefreshShownValue();
    }

    private void GetResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + " hz";
            options.Add(option);

            if (playerPrefResolutionKey != null)
            {
                currentResolutionIndex = selectedResolutionIndex;
            }
            else if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void GetFullScreen()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            fullScreenToggle.isOn = true;
        }
        else
        {
            fullScreenToggle.isOn = false;
        }
    }

    private void SetResolution()
    {
        Resolution selectedResolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

        PlayerPrefs.SetInt(playerPrefResolutionKey, resolutionDropdown.value);
    }

    private void SetQuality()
    {
        int selectedId = 0;
        string selectedSetting = qualitySettings[qualityDropDown.value];

        switch (selectedSetting)
        {
            case "Very Low":
                selectedId = 0;
                break;
            case "Low":
                selectedId = 1;
                break;
            case "Medium":
                selectedId = 2;
                break;
            case "High":
                selectedId = 3;
                break;
            case "Very High":
                selectedId = 4;
                break;
            case "Ultra":
                selectedId = 5;
                break;
            default:
                Debug.LogError("Unhandled Graphics Quality Selected");
                break;
        }

        QualitySettings.SetQualityLevel(selectedId, true);
        PlayerPrefs.SetInt(playerPrefQualityKey, selectedId);
    }

    private void SetFullScreen()
    {
        if (fullScreenToggle.isOn)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        Debug.Log(fullScreenToggle.isOn);
    }

    #endregion

    public void ApplyOptions()
    {
        SetResolution();
        SetQuality();
        SetFullScreen();
    }
}
