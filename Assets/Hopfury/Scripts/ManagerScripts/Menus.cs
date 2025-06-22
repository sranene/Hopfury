using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class Menus : MonoBehaviour {

    public GameObject mainMenuUI;
    public GameObject settingsMenu;
    public GameObject levelSelectUI;
    public GameObject gameplayUI;
    public GameObject diamondUI;
    public GameObject levelCompleteUI;
    public GameObject tutorialCompleteUI;
    public GameObject gameOverMenuUI;

    public GameObject dialogueBox;  // O GameObject pai que tem o script DialogueBoxController
    public GameObject tapHintUI;
    public GameObject swipeHintUI;
    private DialogueBoxController dialogueBoxController;

    private bool allLevelsUnlocked = false;
    public Button[] levelButtons; // Associa os botões na inspector

    //public GameObject pauseButton;
    public Toggle vibrationButton;
    public Slider volumeSlider;
    private AudioSource buttonSound;
    private AudioSource tutorialSound;

    [SerializeField]
    private InputField playerNameInputField = null;
    [SerializeField]
    private Button saveButton = null;

    private const string PlayerNameKey = "PlayerName";

    void Start() 
    {
        // Cria o GameSessionManager se ele ainda não existir na cena
        if (GameSessionManager.Instance == null)
        {
            GameObject sessionManagerObject = new GameObject("GameSessionManager");
            sessionManagerObject.AddComponent<GameSessionManager>();
        }

        buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource> ();
        tutorialSound = GameObject.Find("TutorialSound").GetComponent<AudioSource> ();
        if (AudioManager.Instance == null)
        {
            GameObject audioManagerObject = new GameObject("AudioManager");
            audioManagerObject.AddComponent<AudioManager>();
        }


        if (GameManager.Instance == null)
        {
            GameObject gameManagerObject = new GameObject("GameManager");
            gameManagerObject.AddComponent<GameManager>();
        }
        if (TouchInputManager.Instance == null)
        {
            GameObject touchInputManagerObject = new GameObject("TouchInputManager");
            touchInputManagerObject.AddComponent<TouchInputManager>();
        }
        // Configura o ouvinte para o botão de salvar
        saveButton.onClick.AddListener(SavePlayerName);

    }

    void OnEnable()
    {
        if (playerNameInputField == null)
        {
            GameSessionManager.Instance.LogToFile("playerNameInputField is not assigned!");
            return;
        }

        playerNameInputField.text = PlayerPrefs.GetString("PlayerName", "Player");
        GameSessionManager.Instance.LogToFile("Loaded player name from PlayerPrefs.");
    }

    private void SavePlayerName()
    {
        string newName = playerNameInputField.text;

        if (!string.IsNullOrEmpty(newName))
            {
            // Salva o novo nome no PlayerPrefs
            PlayerPrefs.SetString(PlayerNameKey, newName);
            PlayerPrefs.Save(); // Garante que os dados sejam salvos imediatamente

            // Feedback para o jogador
            GameSessionManager.Instance.LogToFile("Username updated to: " + newName);
            Debug.Log("Nome guardado nos PlayerPrefs: " + newName);
        }
        else
        {
            GameSessionManager.Instance.LogToFile("Invalid username.");
        }
    }

        /////////////
        

    // Ativa o diálogo com o texto do índice 'index'
    public void ShowDialogue(int index)
    {
        tutorialSound.Play();

        if (dialogueBoxController == null && dialogueBox != null)
            dialogueBoxController = dialogueBox.GetComponent<DialogueBoxController>();

        if (dialogueBoxController != null)
        {
            dialogueBox.SetActive(true);
            dialogueBoxController.ShowDialogue(index);
        }
        else
        {
            Debug.LogWarning("DialogueBoxController não encontrado!");
        }
    }

    // Esconde o diálogo
    public void HideDialogue()
    {
        //tutorialSound.Play();
        if (dialogueBox != null)
            dialogueBox.SetActive(false);
    }

    // Ativa o hint de tap
    public void ShowTapHint()
    {
        tutorialSound.Play();
        if (tapHintUI != null)
            tapHintUI.SetActive(true);
    }

    // Desativa o hint de tap
    public void HideTapHint()
    {
        //tutorialSound.Play();
        if (tapHintUI != null)
            tapHintUI.SetActive(false);
    }

    // Ativa o hint de swipe
    public void ShowSwipeHint()
    {
        //tutorialSound.Play();
        if (swipeHintUI != null)
            swipeHintUI.SetActive(true);
    }

    // Desativa o hint de swipe
    public void HideSwipeHint()
    {
        //tutorialSound.Play();
        if (swipeHintUI != null)
            swipeHintUI.SetActive(false);
    }

    // Função para esconder ambos hints (tap e swipe)
    public void HideAllHints()
    {
        //tutorialSound.Play();
        if (tapHintUI != null) tapHintUI.SetActive(false);
        if (swipeHintUI != null) swipeHintUI.SetActive(false);
    }

    /////////////
    ///

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

    public void LoadTutorial()
    {
        GameObject level = Instantiate(Resources.Load("Levels/Level" + Vars.currentLevel, typeof(GameObject))) as GameObject;
        level.name = "Level" + Vars.currentLevel.ToString();
        
        //GameSessionManager.Instance.StartNewSession(level);

        Vars.cameraMaxYPos = -3;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.GetComponent<CameraFollow> ().player = level.transform.Find("Player tutorial").gameObject;

        // Tocar música
        AudioManager.Instance.PlayBackgroundMusicForLevel("10", 0.2f);

        mainMenuUI.SetActive(false);
        levelSelectUI.SetActive(false);
        gameplayUI.SetActive(true);
        levelCompleteUI.SetActive(false);
        tutorialCompleteUI.SetActive(false);
        //diamondUI.SetActive(true);
        //pauseButton.SetActive(true);

        // Resetar os diamantes ao iniciar o nível
        DiamondUIController diamondUI = FindObjectOfType<DiamondUIController>();
        if (diamondUI != null)
        {
            diamondUI.ResetDiamonds();
        }

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
        // Se for nível 0 ou 6, carregar o tutorial correspondente
        if (Vars.currentLevel == "0" || Vars.currentLevel == "6")
        {
            LoadTutorial();
            return;
        }

        GameObject level = Instantiate(Resources.Load("Levels/Level" + Vars.currentLevel, typeof(GameObject))) as GameObject;
        level.name = "Level" + Vars.currentLevel.ToString();

        GameSessionManager.Instance.StartNewSession(level); 

        Vars.cameraMaxYPos = -3;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Camera.main.GetComponent<CameraFollow>().player = level.transform.Find("Player").gameObject;

        // Tocar música
        AudioManager.Instance.PlayBackgroundMusicForLevel(Vars.currentLevel, 0.2f);

        mainMenuUI.SetActive(false);
        levelSelectUI.SetActive(false);
        gameplayUI.SetActive(true);
        levelCompleteUI.SetActive(false);
        tutorialCompleteUI.SetActive(false);
        //diamondUI.SetActive(true);
        //pauseButton.SetActive(true);

        // Resetar os diamantes ao iniciar o nível
        DiamondUIController diamondUI = FindObjectOfType<DiamondUIController>();
        if (diamondUI != null)
        {
            diamondUI.ResetDiamonds();
        }
    }


    /*public void ShowPauseMenu() 
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
    } */

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
        levelCompleteUI.SetActive(false);
        gameOverMenuUI.SetActive(false);
        //diamondUI.SetActive(true);
        DestroyActiveLevel();
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
        //diamondUI.SetActive(false);
        //pauseMenuUI.SetActive(false);
        levelCompleteUI.SetActive(false);
        gameOverMenuUI.SetActive(false);
        //pauseButton.SetActive(true);
        gameplayUI.SetActive(false);
        if(GameObject.Find("Ball") != null) 
        {
            Destroy(GameObject.Find("Ball"));
        }
        DestroyActiveLevel();
    }

    public void LevelComplete() 
    {
        //ALTERAÇÃO
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.EndCurrentSession();
        }
        //ALTERAÇÃO

        int currentLevel = Int32.Parse(Vars.currentLevel);
        if(PlayerPrefs.GetInt("LevelUnlock") < currentLevel + 1) 
        {
            PlayerPrefs.SetInt("LevelUnlock", currentLevel + 1);
        }
        Invoke("ShowLevelCompleteMenu", 1f);
        // Tocar música
        AudioManager.Instance.StopBackgroundMusic();
    }

    private void ShowLevelCompleteMenu() 
    {
        levelCompleteUI.SetActive(true);
        //diamondUI.SetActive(false);
        //pauseButton.SetActive(false);
    }

    public void ToggleLevelLocks()
    {
        buttonSound.Play();

        allLevelsUnlocked = !allLevelsUnlocked;

        if (allLevelsUnlocked)
        {
            // Desbloqueia todos os níveis (níveis 0 a 9)
            for (int i = 0; i < 10; i++)
            {
                PlayerPrefs.SetInt($"Level{i}", 1);
            }
            // Guarda o maior nível desbloqueado (pode usar 9)
            PlayerPrefs.SetInt("LevelUnlock", 9);
        }
        else
        {
            // Bloqueia todos exceto o nível 0
            for (int i = 1; i < 10; i++)
            {
                PlayerPrefs.SetInt($"Level{i}", 0);
            }
            PlayerPrefs.SetInt("LevelUnlock", 0);
        }

        PlayerPrefs.Save();

        UpdateLevelButtons(); // Atualiza os botões do menu para refletir as alterações
    }

    public void UpdateLevelButtons()
    {
        int maxUnlocked = PlayerPrefs.GetInt("LevelUnlock", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool unlocked = i <= maxUnlocked;
            // Liga o botão só se o nível estiver desbloqueado (i <= maxUnlocked)
            levelButtons[i].interactable = (i <= maxUnlocked);

            // Procura o objeto do cadeado (lock) filho e ativa se bloqueado
            Transform lockIcon = levelButtons[i].transform.Find("Lock");
            if (lockIcon != null)
            {
                lockIcon.gameObject.SetActive(!unlocked);
            }

            // Mostrar ou esconder o número do nível (Text ou TMP_Text)
            Transform text = levelButtons[i].transform.Find("Text");
            if (text != null)
            {
                text.gameObject.SetActive(unlocked);
            }

        }

    }


    public void NextLevelAnimation() //This will start the fade in-fade out transition animation
    {
        buttonSound.Play();
        GetComponent<MenuTransitionAnimation> ().menu = 5;
        GetComponent<MenuTransitionAnimation> ().enabled = true;
    }

    public void GameOver() 
    {
        //ALTERAÇÃO
        if (GameSessionManager.Instance != null)
        {
            GameSessionManager.Instance.EndCurrentSession();
        }
        //ALTERAÇÃO
        Invoke("ShowGameOverMenu", 2f);
    }

    private void ShowGameOverMenu() 
    {
        AudioManager.Instance.StopBackgroundMusic();
        gameOverMenuUI.SetActive(true);
    }

    public void NextLevel() 
    {
        Vars.currentLevel = "" + (Int32.Parse(Vars.currentLevel) + 1);

        DestroyActiveLevel();

        LoadLevel();
    }

    public void DestroyActiveLevel()
    {
        Regex levelNamePattern = new Regex(@"^Level\d{1,2}$"); // "Level" seguido de 1 ou 2 dígitos

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (levelNamePattern.IsMatch(go.name))
            {
                Destroy(go);
            }
        }
    }


    public void ExitTheGame() 
    {
        buttonSound.Play();
        Application.Quit();
    }

}
