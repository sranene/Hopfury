using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menus : MonoBehaviour {

    public GameObject mainMenuUI;
    public GameObject settingsMenu;
    public GameObject levelSelectUI;
    public GameObject gameplayUI;
    public GameObject pauseMenuUI;
    public GameObject levelCompleteUI;
    public GameObject gameOverMenuUI;
    public GameObject pauseButton;
    public Toggle vibrationButton;
    public Slider volumeSlider;
    private AudioSource buttonSound;

    void Start() 
    {
        buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource> ();
    }

    public void ShowSettingsMenu() 
    {
        buttonSound.Play();
        settingsMenu.SetActive(true);
    }

    public void SetVibration() //This is called then the user checks or unchecks vibration option is the settings menu
    {
        if(vibrationButton.isOn) 
        {
            PlayerPrefs.SetInt("Vibration", 1);
        }else 
        {
            PlayerPrefs.SetInt("Vibration", 0);
        }
    }

    public void SetVolume() //This is called when the value on the volume slider is changed
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void HideSettingsMenu() 
    {
        buttonSound.Play();
        settingsMenu.SetActive(false);
    }

    public void ShowLevelSelectMenuAnimation() //This will start the fade in-fade out transition animation
    {
        buttonSound.Play();
        GetComponent<MenuTransitionAnimation> ().menu = 0;
        GetComponent<MenuTransitionAnimation> ().enabled = true;
    }

    public void HideLevelSelectMenuAnimation() //This will start the fade in-fade out transition animation
    {
        buttonSound.Play();
        GetComponent<MenuTransitionAnimation> ().menu = 1;
        GetComponent<MenuTransitionAnimation> ().enabled = true;
    }

    public void ShowLevelSelectMenu() 
    {
        mainMenuUI.SetActive(false);
        levelSelectUI.SetActive(true);
    }

    public void HideLevelSelectMenu() 
    {
        mainMenuUI.SetActive(true);
        levelSelectUI.SetActive(false);
    }

    public void LevelLoadAnimation () //This will start the fade in-fade out transition animation
    {
        buttonSound.Play();
        Vars.currentLevel = EventSystem.current.currentSelectedGameObject.name;
        GetComponent<MenuTransitionAnimation> ().menu = 2;
        GetComponent<MenuTransitionAnimation> ().enabled = true;
    }
   
    public void LoadLevel() 
    {
        GameObject level = Instantiate(Resources.Load("Levels/Level" + Vars.currentLevel, typeof(GameObject))) as GameObject;
        level.name = "Level";
        Vars.cameraMaxYPos = -3;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.GetComponent<CameraFollow> ().player = level.transform.Find("Player").gameObject;
        mainMenuUI.SetActive(false);
        levelSelectUI.SetActive(false);
        gameplayUI.SetActive(true);
        levelCompleteUI.SetActive(false);
        pauseButton.SetActive(true);
	}

    public void ShowPauseMenu() 
    {
        buttonSound.Play();
        Time.timeScale = 0;
        pauseMenuUI.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void HidePauseMenu() 
    {
        buttonSound.Play();
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        pauseButton.SetActive(true);
        CancelAllInvokes();
    }

    public void RestartLevelAnimation() //This will start the fade in-fade out transition animation
    {
        buttonSound.Play();
        Time.timeScale = 1;
        GetComponent<MenuTransitionAnimation> ().menu = 3;
        GetComponent<MenuTransitionAnimation> ().enabled = true;
        CancelAllInvokes();
    }

    private void CancelAllInvokes() 
    {
        CancelInvoke("ShowLevelCompleteMenu");
        CancelInvoke("ShowGameOverMenu");
    }

    public void RestartLevel() 
    {
        Time.timeScale = 1;
        pauseMenuUI.SetActive(false);
        levelCompleteUI.SetActive(false);
        gameOverMenuUI.SetActive(false);
        pauseButton.SetActive(true);
        if(GameObject.Find("Level") != null) 
        {
            Destroy(GameObject.Find("Level"));
        }
        LoadLevel();
    }

    public void ExitToMainMenuAnimation() //This will start the fade in-fade out transition animation
    {
        buttonSound.Play();
        Time.timeScale = 1;
        GetComponent<MenuTransitionAnimation> ().menu = 4;
        GetComponent<MenuTransitionAnimation> ().enabled = true;
        CancelAllInvokes();
    }

    public void ExitToMainMenu() 
    {
        HideLevelSelectMenu();
        pauseMenuUI.SetActive(false);
        levelCompleteUI.SetActive(false);
        gameOverMenuUI.SetActive(false);
        pauseButton.SetActive(true);
        gameplayUI.SetActive(false);
        if(GameObject.Find("Ball") != null) 
        {
            Destroy(GameObject.Find("Ball"));
        }
        if(GameObject.Find("Level") != null) 
        {
            Destroy(GameObject.Find("Level"));
        }
    }

    public void LevelComplete() 
    {
        int currentLevel = Int32.Parse(Vars.currentLevel);
        if(PlayerPrefs.GetInt("LevelUnlock") < currentLevel + 1) 
        {
            PlayerPrefs.SetInt("LevelUnlock", currentLevel + 1);
        }
        Invoke("ShowLevelCompleteMenu", 1f);
    }

    private void ShowLevelCompleteMenu() 
    {
        levelCompleteUI.SetActive(true);
        pauseButton.SetActive(false);
    }

    public void NextLevelAnimation() //This will start the fade in-fade out transition animation
    {
        buttonSound.Play();
        GetComponent<MenuTransitionAnimation> ().menu = 5;
        GetComponent<MenuTransitionAnimation> ().enabled = true;
    }

    public void GameOver() 
    {
        Invoke("ShowGameOverMenu", 1f);
    }

    private void ShowGameOverMenu() 
    {
        gameOverMenuUI.SetActive(true);
    }

    public void NextLevel() 
    {
        Vars.currentLevel = "" + (Int32.Parse(Vars.currentLevel) + 1);
        if(GameObject.Find("Level") != null) Destroy(GameObject.Find("Level"));
        LoadLevel();
    }

    public void ExitTheGame() 
    {
        buttonSound.Play();
        Application.Quit();
    }

}
