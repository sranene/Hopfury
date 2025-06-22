using UnityEngine;

public class RandomTiledBackground : MonoBehaviour
{
    [SerializeField] private Transform followTarget; // Normalmente a Camera
    [SerializeField, Range(0f, 1f)] private float parallaxFactor = 0.5f;

    private Vector3 initialOffset;
    private float maxX; // Posição máxima do followTarget em X

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
                Debug.LogWarning("SpriteRenderer não encontrado no CastleLayer.");
            }
        }
        else
        {
            Debug.LogWarning("CastleLayer não encontrado como filho deste GameObject.");
        }
    }

    void LateUpdate()
    {
        if (followTarget.position.x >= 295f)
        {
            // Trava a movimentação ao atingir o limite
            return;
        }

        // Atualiza o valor máximo de X se o followTarget avançar
        if (followTarget.position.x > maxX)
        {
            maxX = followTarget.position.x;
        }

        // Usa o valor máximo alcançado para calcular a posição do fundo
        Vector3 newPosition = new Vector3(maxX * parallaxFactor + initialOffset.x, transform.position.y, transform.position.z);
        transform.position = newPosition;
    }
}
