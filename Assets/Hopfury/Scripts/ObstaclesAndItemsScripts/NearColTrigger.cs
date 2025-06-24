using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class NearColTrigger : MonoBehaviour
{
    private float timeStart = -1f;
    private float timeEnd = -1f;
    private float timeStimuli = -1f;
    private string obstacleName;

    private float x = -999f;
    private float y = -999f;
    private float width = -999f;

    public float GetTimeStart() => timeStart;
    public float GetTimeEnd() => timeEnd;
    public float GetTimeStimuli() => timeStimuli;
    public string GetName() => obstacleName;

    public float GetX() => x;
    public float GetY() => y;
    //public float GetWidth() => width;


    void Start()
    {
        // Se tiver pai, é filho de um obstáculo
        if (transform.parent != null && transform.parent.name != "LevelObjects")
        {
            obstacleName = transform.parent.name;
            GameSessionManager.Instance.LogToFile($"TEM PAI {obstacleName}");

            Transform endChild = null;

            foreach (Transform child in transform.parent)
            {
                if (child.name.ToLower().Contains("end"))
                {
                    endChild = child;
                    break;
                }
            }

            if (endChild != null)
            {
                x = endChild.position.x;
                GameSessionManager.Instance.LogToFile($"[NearCol] X do filho 'end': {x}");
            }
            else
            {
                GameSessionManager.Instance.LogToFile("[NearCol] Nenhum filho com 'end' no nome foi encontrado.");
            }

            // Para manter o Y também do pai, se quiseres
            y = transform.parent.position.y;

            // OPCIONAL: caso queiras remover esta parte, podes
            BoxCollider2D parentCollider = transform.parent.GetComponent<BoxCollider2D>();
            if (parentCollider != null)
            {
                GameSessionManager.Instance.LogToFile($"[NearCol] Pai tem BoxCollider2D.");
            }
            else
            {
                GameSessionManager.Instance.LogToFile($"[NearCol] Pai não tem BoxCollider2D!");
            }
        }
        else
        {
            obstacleName = gameObject.name;
            GameSessionManager.Instance.LogToFile($"É O PRÓPRIO PAI {obstacleName}");
        }

        // Só corre a coroutine de visibilidade se for NearCol start OU se for o próprio obstáculo
        if (gameObject.name.ToLower().Contains("start") || transform.parent == null)
        {
            StartCoroutine(WaitForSelfOrParentVisibility());
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        if (transform.parent == null)
        {
            // Se for o próprio pai, não regista trigger
            return;
        }

        float playerX = col.transform.position.x;

        if (gameObject.name.ToLower().Contains("start"))
        {
            timeStart = GameSessionManager.Instance.GetElapsedTime();
            GameSessionManager.Instance.LogToFile($"[START] Jogador entrou no trigger. Tempo: {timeStart} na posição X = {playerX}");
        }
        else if (gameObject.name.ToLower().Contains("end"))
        {
            timeEnd = GameSessionManager.Instance.GetElapsedTime();
            GameSessionManager.Instance.LogToFile($"[END] Jogador entrou no trigger. Tempo: {timeEnd} na posição X = {playerX}");
        }
    }


    private IEnumerator WaitForSelfOrParentVisibility()
    {
        Renderer renderer = null;

        // Tenta obter do pai
        if (transform.parent != null)
        {
            renderer = transform.parent.GetComponent<Renderer>();

            // Se o pai não tiver, tenta o filho chamado "Left"
            if (renderer == null)
            {
                Transform leftChild = transform.parent.Find("Left");
                if (leftChild != null)
                {
                    renderer = leftChild.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        GameSessionManager.Instance.LogToFile("[NearCol] Renderer encontrado no filho 'Left'.");
                    }
                }
            }
            else
            {
                GameSessionManager.Instance.LogToFile("[NearCol] Renderer encontrado no pai.");
            }
        }

        // Se ainda não encontrou, tenta no próprio objeto
        if (renderer == null)
        {
            renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                GameSessionManager.Instance.LogToFile("[NearCol] Renderer encontrado no próprio objeto.");
            }
        }

        if (renderer == null)
        {
            GameSessionManager.Instance.LogToFile("[NearCol] Nenhum Renderer encontrado para visibilidade!");
            yield break;
        }

        while (!renderer.isVisible)
        {
            yield return null;
        }

        timeStimuli = GameSessionManager.Instance.GetElapsedTime();
        GameSessionManager.Instance.LogToFile($"[NearCol] Obstáculo ficou visível no tempo: {timeStimuli}");
    }

}
