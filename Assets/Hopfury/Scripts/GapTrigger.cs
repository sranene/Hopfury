using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class GapTrigger : MonoBehaviour
{
    private float timeStart = -1f;
    private float timeEnd = -1f;
    private float timeStimuli = -1f;  // Se quiseres usar, depois ativa
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
    public float GetWidth() => width;

    void Start()
    {
        // Assume sempre que tem pai
        obstacleName = transform.parent.name;

        // Procurar o filho chamado "start" para obter o X e Y dele
        Transform startChild = transform.parent.Find("start");

        if (startChild != null)
        {
            Vector2 pos = startChild.position;
            x = pos.x;
            y = pos.y;

            BoxCollider2D startCollider = startChild.GetComponent<BoxCollider2D>();
            if (startCollider != null)
            {
                width = startCollider.size.x * startChild.localScale.x;
            }

            GameSessionManager.Instance.LogToFile($"[GapTrigger] START filho - X: {x}, Y: {y}, Width: {width}");
        }
        else
        {
            GameSessionManager.Instance.LogToFile("[GapTrigger] Filho 'start' não encontrado!");

            // Fallback: tentar buscar do pai se o filho não existir
            BoxCollider2D parentCollider = transform.parent.GetComponent<BoxCollider2D>();
            if (parentCollider != null)
            {
                Vector2 parentPos = transform.parent.position;
                x = parentPos.x;
                y = parentPos.y;
                width = parentCollider.size.x * transform.parent.localScale.x;

                GameSessionManager.Instance.LogToFile($"[GapTrigger] Pai (fallback) - X: {x}, Y: {y}, Width: {width}");
            }
            else
            {
                GameSessionManager.Instance.LogToFile("[GapTrigger] Pai não tem BoxCollider2D!");
            }
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

            // Procura o irmão 'start' para buscar dados e chamar SetObstacle
            Transform startSibling = transform.parent.Find("start");
            if (startSibling != null)
            {
                var startTrigger = startSibling.GetComponent<GapTrigger>(); // ou o nome do script que usas
                if (startTrigger != null)
                {
                    string name = startTrigger.GetName();
                    float start = startTrigger.GetTimeStart();
                    float stimuli = startTrigger.GetTimeStimuli();
                    float finishTime = GameSessionManager.Instance.GetElapsedTime();
                    float x = startTrigger.GetX();
                    float y = startTrigger.GetY();
                    float width = startTrigger.GetWidth();

                    GameSessionManager.Instance.SetObstacle(name, start, timeEnd, stimuli, finishTime, x, y, width);
                    GameSessionManager.Instance.LogToFile("[END] SetObstacle chamado com dados do irmão 'start'.");
                }
                else
                {
                    GameSessionManager.Instance.LogToFile("[END] GapTrigger não encontrado no irmão 'start'.");
                }
            }
            else
            {
                GameSessionManager.Instance.LogToFile("[END] Irmão 'start' não encontrado.");
            }
        }
    }

    private IEnumerator WaitForSelfOrParentVisibility()
    {
        Renderer renderer = transform.parent != null
            ? transform.parent.GetComponent<Renderer>()
            : GetComponent<Renderer>();

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
