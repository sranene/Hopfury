using UnityEngine;

public class RandomTiledBackground : MonoBehaviour
{
    [SerializeField] private Transform followTarget; // Normalmente a Camera
    [SerializeField, Range(0f, 1f)] private float parallaxFactor = 0.5f;

    private Vector3 initialOffset;
    private float maxX; // Posi��o m�xima do followTarget em X

    void Start()
    {
        if (followTarget == null)
            followTarget = Camera.main.transform;

        initialOffset = transform.position - followTarget.position;
        maxX = followTarget.position.x;

        // Ajusta o sortingOrder do filho "CastleLayer"
        Transform castleLayer = transform.Find("CastleLayer");
        if (castleLayer != null)
        {
            SpriteRenderer sr = castleLayer.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = -10;
            }
            else
            {
                Debug.LogWarning("SpriteRenderer n�o encontrado no CastleLayer.");
            }
        }
        else
        {
            Debug.LogWarning("CastleLayer n�o encontrado como filho deste GameObject.");
        }
    }

    void LateUpdate()
    {
        if (followTarget.position.x >= 295f)
        {
            // Trava a movimenta��o ao atingir o limite
            return;
        }

        // Atualiza o valor m�ximo de X se o followTarget avan�ar
        if (followTarget.position.x > maxX)
        {
            maxX = followTarget.position.x;
        }

        // Usa o valor m�ximo alcan�ado para calcular a posi��o do fundo
        Vector3 newPosition = new Vector3(maxX * parallaxFactor + initialOffset.x, transform.position.y, transform.position.z);
        transform.position = newPosition;
    }
}
