using UnityEngine;

public class RestStationChild : MonoBehaviour
{
    private RestStation restStation;

    private void Start()
    {
        restStation = GetComponentInParent<RestStation>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name == "Player" || col.gameObject.name == "Player tutorial")
        {
            Vector2 direction = (col.transform.position - transform.position).normalized;

            if (gameObject.name == "Right Box")
            {
                Debug.Log($" entrou right box");
                // Verifica se bateu do lado esquerdo do RightBox
                if (direction.x < -0.5f)
                {
                    restStation.SwapSprites();
                }
            }
            else if (gameObject.name == "Left Box")
            {
                Debug.Log($" entrou left box ");
                // Verifica se bateu do lado direito do LeftBox
                if (direction.x > 0.5f)
                {
                    restStation.SwapSprites();
                }
            }
        }
    }
}
