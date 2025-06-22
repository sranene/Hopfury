using UnityEngine;

public class FloorChallenge : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.EnterChallengeMode();
        }
    }
}
