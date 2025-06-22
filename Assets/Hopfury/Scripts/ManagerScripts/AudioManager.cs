using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource audioSource; // Variável global para armazenar o AudioSource


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

    // Função para tocar a música de fundo de acordo com o nível
    public void PlayBackgroundMusicForLevel(string level, float volume = 1.0f)
    {
        string trackName = "6. Track 6"; // Valor padrão para níveis além do Level 1
        Debug.Log($"level {level}");

        // Lógica para definir a música dependendo do nível
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



        // Criar o caminho do arquivo para a música
        string trackPath = "8Bit Music - 062022/" + trackName;

        // Obter ou adicionar o AudioSource
        if (audioSource == null)
        {
            audioSource = Camera.main.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = Camera.main.gameObject.AddComponent<AudioSource>(); // Adiciona o AudioSource se não existir
            }

        }

        // Carregar o áudio a partir do caminho fornecido
        AudioClip backgroundMusic = Resources.Load<AudioClip>(trackPath);

        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; // Configura para a música se repetir
            audioSource.volume = volume; // Ajusta o volume com o valor fornecido
            audioSource.Play(); // Começa a tocar a música
        }
        else
        {
            Debug.LogError("Não foi possível carregar a música. Verifique o caminho do arquivo.");
        }
    }

    // Função para parar a música
    public void StopBackgroundMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // Para a música
        }
        else
        {
            Debug.LogWarning("AudioSource não encontrado para parar a música.");
        }
    }
}
