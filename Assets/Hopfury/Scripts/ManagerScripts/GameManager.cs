using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;
    public bool challengeMode = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional se quiseres manter o GameManager entre cenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsInChallengeMode()
    {
        return challengeMode;
    }

    public void EnterChallengeMode()
    {
        Debug.Log($"challengeMode = true");
        challengeMode = true;
    }

    public void ExitChallengeMode()
    {
        challengeMode = false;
    }

}
