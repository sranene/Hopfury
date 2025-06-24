using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    private bool triggered = false;  // controla se já foi acionado

    void OnTriggerEnter2D(Collider2D col)
    {
        if (triggered) return;  // ignora se já foi acionado antes

        if (!col.CompareTag("Player")) return;

        triggered = true;  // marca que já foi acionado

        Transform parent = transform.parent;

        NearColTrigger startTrigger = null;
        NearColTrigger endTrigger = null;

        foreach (Transform child in parent)
        {
            if (child.name.ToLower().Contains("start"))
            {
                startTrigger = child.GetComponent<NearColTrigger>();
            }
            else if (child.name.ToLower().Contains("end"))
            {
                endTrigger = child.GetComponent<NearColTrigger>();
            }
        }

        if (startTrigger != null && endTrigger != null)
        {
            float start = startTrigger.GetTimeStart();
            float stimuli = startTrigger.GetTimeStimuli();
            string name = startTrigger.GetName();

            float end = endTrigger.GetTimeEnd();
            float finishTime = GameSessionManager.Instance.GetElapsedTime();

            // Buscar posição e largura do obstáculo (a partir do trigger start)
            float x = startTrigger.GetX();
            float y = startTrigger.GetY();
            //float width = startTrigger.GetWidth();
            float width = -1;

            GameSessionManager.Instance.LogToFile($"[Finish] SAVing obstacle com posição X={x}, Y={y}, Width={width}");
            GameSessionManager.Instance.SetObstacle(name, start, end, stimuli, finishTime, x, y, width);
        }
        else
        {
            GameSessionManager.Instance.LogToFile("[Finish] Trigger start ou end não encontrado!");
        }
    }
}
