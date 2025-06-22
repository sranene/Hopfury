using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnlockLevel : MonoBehaviour {
	//This script is attached on each button that is used to select the level in the level select menu
	void OnEnable () //This method is called when object is enabled (SetActive() method is set to true)
    {
        int gameLevel = Int32.Parse(this.gameObject.name);

        if (PlayerPrefs.GetInt("LevelUnlock") >= gameLevel) //It will check whether that level is unlocked
        {
            this.transform.Find("Lock").gameObject.SetActive(false);

            foreach (Transform child in transform)
            {
                if (child.name == "Text")
                    child.gameObject.SetActive(true);
            }

            GetComponent<Button>().interactable = true;
        }
        else //If level is not unlocked GetComponent<Button>().interactable will not be set to true and button will not be interactable
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Text")
                    child.gameObject.SetActive(false);
            }
        }
    }
}
