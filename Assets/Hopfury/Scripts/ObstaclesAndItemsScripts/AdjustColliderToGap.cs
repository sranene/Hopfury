using UnityEngine;

public class AdjustColliderToGap : MonoBehaviour
{
    public float playerHalfWidth = 0.35f; // Metade do tamanho do player em X

    void Start()
    {
        BoxCollider2D mainCollider = GetComponent<BoxCollider2D>();
        if (mainCollider == null)
        {
            Debug.LogWarning("BoxCollider2D não encontrado no objeto pai.");
            return;
        }

        float colWidth = mainCollider.size.x;
        float colOffsetX = mainCollider.offset.x;
        float colCenterX = transform.position.x + colOffsetX;

        float leftEdge = colCenterX - (colWidth / 2f);
        float rightEdge = colCenterX + (colWidth / 2f);

        Transform startChild = transform.Find("start");
        Transform endChild = transform.Find("end");

        if (startChild != null)
        {
            Vector3 pos = startChild.position;
            pos.x = leftEdge - playerHalfWidth;
            startChild.position = pos;
        }

        if (endChild != null)
        {
            Vector3 pos = endChild.position;
            pos.x = rightEdge + playerHalfWidth;
            endChild.position = pos;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        // Procura o filho "start"
        Transform startChild = transform.Find("start");
        if (startChild == null)
        {
            Debug.LogWarning("Filho 'start' não encontrado.");
            return;
        }

        GapTrigger gapTrigger = startChild.GetComponent<GapTrigger>();
        if (gapTrigger == null)
        {
            Debug.LogWarning("GapTrigger não encontrado no filho 'start'.");
            return;
        }

        // Recolhe dados do filho start
        string name = gapTrigger.GetName();
        float start = gapTrigger.GetTimeStart();
        float end = GameSessionManager.Instance.GetElapsedTime();
        float stimuli = gapTrigger.GetTimeStimuli();
        float finishTime = GameSessionManager.Instance.GetElapsedTime();
        float x = gapTrigger.GetX();
        float y = gapTrigger.GetY();
        float width = gapTrigger.GetWidth();

        GameSessionManager.Instance.LogToFile("[AdjustColliderToGap] Chamando SetObstacle com dados do filho 'start'.");

        GameSessionManager.Instance.SetObstacle(name, start, end, stimuli, finishTime, x, y, width);
    }
}
