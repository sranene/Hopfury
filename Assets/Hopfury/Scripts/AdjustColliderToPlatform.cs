using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AdjustColliderToPlatform : MonoBehaviour
{
    void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            Vector2 currentSize = collider.size;
            Vector2 currentOffset = collider.offset;
            Vector2 spriteSize = sr.size;

            // Ajustar só no eixo X
            collider.size = new Vector2(spriteSize.x, currentSize.y);
            collider.offset = new Vector2(0f, currentOffset.y);

            // Reposicionar o filho "Spike" para a esquerda da plataforma, com um pequeno deslocamento
            Transform spike = transform.Find("Spike");
            if (spike != null)
            {
                float extraOffset = 0.3f;
                float leftEdge = transform.position.x - spriteSize.x / 2f - extraOffset;
                Vector3 spikePos = spike.position;
                spike.position = new Vector3(leftEdge, spikePos.y, spikePos.z);
            }
            else
            {
                Debug.LogWarning("Filho 'Spike' não encontrado em " + gameObject.name);
            }

            // Ajustar posição dos NearCol start e end
            Transform nearColStart = transform.Find("NearCol start");
            Transform nearColEnd = transform.Find("NearCol end");

            if (nearColStart != null)
            {
                float platformLeftEdge = transform.position.x - spriteSize.x / 2f;
                float startX = platformLeftEdge - 2.3f + 0.35f;
                Vector3 startPos = nearColStart.position;
                nearColStart.position = new Vector3(startX, startPos.y, startPos.z);
            }
            else
            {
                Debug.LogWarning("Filho 'NearCol start' não encontrado em " + gameObject.name);
            }

            if (nearColStart != null && nearColEnd != null)
            {
                Vector3 startPos = nearColStart.position;
                nearColEnd.position = new Vector3(startPos.x + spriteSize.x, nearColEnd.position.y, nearColEnd.position.z);
            }
            else if (nearColEnd == null)
            {
                Debug.LogWarning("Filho 'NearCol end' não encontrado em " + gameObject.name);
            }

            // Posicionar FinishCol
            Transform finishCol = transform.Find("FinishCol");
            if (finishCol != null)
            {
                float rightEdge = transform.position.x + spriteSize.x / 2f;
                float finishX = rightEdge + 1f;
                finishCol.position = new Vector3(finishX, finishCol.position.y, finishCol.position.z);
            }
            else
            {
                Debug.LogWarning("Filho 'FinishCol' não encontrado em " + gameObject.name);
            }
        }
        else
        {
            Debug.LogWarning("SpriteRenderer não encontrado em " + gameObject.name);
        }
    }
}
