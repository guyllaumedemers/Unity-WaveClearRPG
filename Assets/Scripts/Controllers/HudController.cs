using Characters;
using Characters.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    public Canvas menuCanvas;
    public Image healthBar;

    Text winLoseText;
    Text mimicText;
    Text beholderText;

    Text waveTimerText;

    Text strText;
    Text defText;

    bool hasWon;
    bool hasLost;

    private void Awake()
    {
        mimicText = GameObject.Find("MimicText").GetComponent<Text>();
        beholderText = GameObject.Find("BeholderText").GetComponent<Text>();

        waveTimerText = GameObject.Find("WaveTimerText").GetComponent<Text>();

        strText = GameObject.Find("StrengthText").GetComponent<Text>();
        defText = GameObject.Find("DefenseText").GetComponent<Text>();
    }

    public void UpdatePlayerHud(Stats stats)
    {
        healthBar.fillAmount = stats.currentHealth / stats.maxHealth;

        mimicText.text = GameController.Instance._nbMimic.ToString();
        beholderText.text = GameController.Instance._nbBeholder.ToString();
        //enemiesText.text = "Enemies Remaining: " + GameController.Instance.enemiesLeft + "/" + GameController.Instance.maxEnemiesPerWave;

        if (GameController.Instance._waveReady)
        {
            waveTimerText.enabled = false;
        }
        else
        {
            waveTimerText.enabled = true;

            if (GameController.Instance._currentWave == 1)
            {
                waveTimerText.text = "Game starting in " + (int)GameController.Instance._waveTimeRemaining + "...";
            }
            else
            {
                waveTimerText.text = "Wave " + GameController.Instance._currentWave + " starting in " + (int)GameController.Instance._waveTimeRemaining + "...";
            }
        }

        strText.text = "Str: " + Player.Instance.playerStats.strength;
        defText.text = "Def: " + Player.Instance.playerStats.defense;

        hasWon = GameController.Instance.CheckWinConditions();
        hasLost = GameController.Instance.CheckLoseConditions();

        if (hasWon)
        {
            ShowWinMenu();
        }
        else if (hasLost)
        {
            ShowLoseMenu();
        }
    }
    public void ShowLoseMenu()
    {
        menuCanvas.gameObject.SetActive(true);
        winLoseText = GameObject.Find("MenuPanel").GetComponentInChildren<Text>(true);
        winLoseText.color = Color.red;
        winLoseText.text = "You have died.";
    }

    public void ShowWinMenu()
    {
        menuCanvas.gameObject.SetActive(true);
        winLoseText = GameObject.Find("MenuPanel").GetComponentInChildren<Text>(true);
        winLoseText.color = Color.green;
        winLoseText.text = "You have won!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowMenu()
    {
        if (menuCanvas.gameObject.activeSelf == false)
        {
            Time.timeScale = 0;
            GameController.Instance._isPaused = true;
            menuCanvas.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            GameController.Instance._isPaused = false;
            menuCanvas.gameObject.SetActive(false);
        }
    }
}
