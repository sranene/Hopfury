using UnityEngine;

public class DiamondTrigger : MonoBehaviour
{
    private AudioSource diamondSound;
    private DiamondUIController uiController;

    void Start()
    {
        diamondSound = GameObject.Find("DiamondSound").GetComponent<AudioSource>();
        uiController = GameObject.FindObjectOfType<DiamondUIController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            diamondSound.Play();
            gameObject.SetActive(false);  // Desativa o diamante

            DiamondUIController diamondUI = FindObjectOfType<DiamondUIController>();
            if (diamondUI != null)
            {
                diamondUI.AddDiamond();
            }
            GameSessionManager.Instance.GotDiamond();
            GameSessionManager.Instance.LogToFile("Player apanhou diamante enquanto isTrigger: " + other.isTrigger);
        }
    }

}
