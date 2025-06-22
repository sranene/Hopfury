using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource audioSource; // Vari�vel global para armazenar o AudioSource


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Fun��o para tocar a m�sica de fundo de acordo com o n�vel
    public void PlayBackgroundMusicForLevel(string level, float volume = 1.0f)
    {
        string trackName = "6. Track 6"; // Valor padr�o para n�veis al�m do Level 1
        Debug.Log($"level {level}");

        // L�gica para definir a m�sica dependendo do n�vel
        if (level == "0") // tutorial
        {
            trackName = "1. Track 1";
        } else if (level == "1")
        {
            trackName = "1. Track 1";
        } else if (level == "2")
        {
            trackName = "3. Track 3";
        } else if (level == "3")
        {
            trackName = "9. Track 9";
        } else if (level == "4")
        {
            trackName = "8. Track 8";
        } else if (level == "5")
        {
            trackName = "10. Track 10";
        } else if (level == "6") // tutorial
        {
            trackName = "1. Track 1";
        } else if (level == "7")
        {
            trackName = "5. Track 5";
        } else if (level == "8")
        {
            trackName = "2. Track 2";
        } else if (level == "9")
        {
            trackName = "4. Track 4";
        } else if (level == "10")
        {
            trackName = "6. Track 6";
        } else if (level == "11")
        {
            trackName = "7. Track 7";
        } 



        // Criar o caminho do arquivo para a m�sica
        string trackPath = "8Bit Music - 062022/" + trackName;

        // Obter ou adicionar o AudioSource
        if (audioSource == null)
        {
            audioSource = Camera.main.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = Camera.main.gameObject.AddComponent<AudioSource>(); // Adiciona o AudioSource se n�o existir
            }

        }

        // Carregar o �udio a partir do caminho fornecido
        AudioClip backgroundMusic = Resources.Load<AudioClip>(trackPath);

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Configura para a m�sica se repetir
            audioSource.volume = volume; // Ajusta o volume com o valor fornecido
            audioSource.Play(); // Come�a a tocar a m�sica
        }
        else
        {
            Debug.LogError("N�o foi poss�vel carregar a m�sica. Verifique o caminho do arquivo.");
        }
    }

    // Fun��o para parar a m�sica
    public void StopBackgroundMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // Para a m�sica
        }
        else
        {
            Debug.LogWarning("AudioSource n�o encontrado para parar a m�sica.");
        }
    }
}
