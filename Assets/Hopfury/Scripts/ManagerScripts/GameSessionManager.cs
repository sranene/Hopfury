using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


// Tipos de dados usados
[Serializable]
public class PlayerData
{
    public string name;
    public int levelUnlock = 0;
    public int unlockNextLevel = 0;
    public List<SessionData> sessions = new List<SessionData>();
}

[Serializable]
public class SessionData
{
    public string level;
    public string startTime;
    public string endTime;
    public float timeOfDeath;
    public List<ObstacleData> obstacles;
}

[Serializable]
public class PlatformData
{
    public int id;
    public string name;
    // Outros dados relevantes
}

[System.Serializable]
public class ObstacleData
{
    public string id;          // Identificador único para o obs
    public string name;
    public float timeStimuli; // Quando o obs apareceu
    public float timeIntStart; // Quando pode começar o tap
    public float timeIntEnd;   // Quando deixa de poder dar tap
    public float finishTime;   // Quando o utilizador morre ou ultrapassa completamente o obstáculo
    public float x;
    public float y;
    public float width;
    public List<Tap> taps = new List<Tap>();
}

[System.Serializable]
public class Tap
{
    public int id;             // Identificador único para o toque
    public bool isTap;
    public float x;            // Coordenada X do toque
    public float y;            // Coordenada Y do toque
    public float timeStart;    // Momento em que o toque começou
    public float timeHold;     // Tempo que durou o toque
    public float timeEnd;      // Momento em que o toque terminou
    public float pressure;    // Pressão do toque (depende do dispositivo)
    public float radius;
    public float radiusVariance;
    public float square_x;
    public float square_y;
    public float landedTime;
}

[Serializable]
public class PlayerList
{
    public List<PlayerData> players = new List<PlayerData>();
}


public class GameSessionManager : MonoBehaviour
{
    public static GameSessionManager Instance { get; private set; }

    private string logFilePath;

    private PlayerList playerList = new PlayerList();

    private PlayerData currentPlayer;
    private SessionData currentSession;
    private Dictionary<string, ObstacleData> obstacles = new Dictionary<string, ObstacleData>();
    private List<Tap> pendingTaps = new List<Tap>();

    private float gameStartTime = 0f;
    private int obstacleId = 0;

    private Stopwatch stopwatch;


    private void Awake()
    {
        logFilePath = Path.Combine(Application.persistentDataPath, "game_logs.txt");

        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LogToFile("GameSessionManager initialized successfully.");
        }
        else
        {
            LogToFile("Another GameSessionManager instance was found. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        LoadPlayersFromJson();
    }

    public void SetObstacle(string name, float timeStart, float timeEnd, float timeStimuli, float fTime, float x, float y, float width)
    {
        string id = (obstacleId++).ToString();

        ObstacleData newObstacle = new ObstacleData
        {
            id = id,
            name = name,
            timeStimuli = timeStimuli,
            timeIntStart = timeStart,
            timeIntEnd = timeEnd,
            finishTime = fTime,
            x = x,
            y = y,
            width = width
        };

        obstacles.Add(id, newObstacle);

        LogToFile($"[GameSessionManager] Obstacle {name} registado com ID {id}");

        // Chamada ao sistema de validação ou emparelhamento de interações
        CheckPendingTapsForObstacle(id);
    }


    private void CheckPendingTapsForObstacle(string obsId)
    {
        // Garante que o obstáculo existe
        if (!obstacles.ContainsKey(obsId))
        {
            LogToFile($"Obstacle {obsId} not found.");
            return;
        }

        // Obtém os dados do obstacle
        ObstacleData obsData = obstacles[obsId];

        // Define o intervalo de tempo do obstáculo
        float startTime = obsData.timeStimuli;
        float endTime = obsData.finishTime;

        // Filtra os taps que ocorreram durante o tempo do obstáculo
        List<Tap> tapsDuringObstacle = pendingTaps
            .Where(tap => tap.timeStart >= startTime && tap.timeStart <= endTime)
            .ToList();

        // Adiciona esses taps ao obstáculo
        obsData.taps.AddRange(tapsDuringObstacle);
        currentSession?.obstacles.Add(obsData);

        LogToFile($"Obstacle {obsId} registered with {obsData.taps.Count} taps between {startTime} and {endTime}.");
    }


    public void RegisterPendingTap(Tap tap)
    {
        pendingTaps.Add(tap);
    }

    public void IncrementLevelUnlock()
    {
        if (currentPlayer != null)
        {
            currentPlayer.levelUnlock++;
            LogToFile($"[Progress] {currentPlayer.name} -> levelUnlock = {currentPlayer.levelUnlock}");
        }
        else
        {
            LogToFile("[Progress] No current player found. Cannot increment levelsCompleted.");
        }
    }

    public PlayerData GetPlayer(string playerName)
    {
        if (playerList != null && playerList.players != null)
        {
            PlayerData foundPlayer = playerList.players.Find(p => p.name == playerName);

            if (foundPlayer != null)
            {
                LogToFile($"[GetPlayer] Found player: {playerName}");
                return foundPlayer;
            }
            else
            {
                LogToFile($"[GetPlayer] Player not found: {playerName}");
                return null;
            }
        }
        else
        {
            LogToFile("[GetPlayer] Player list is null or empty.");
            return null;
        }
    }




    // Guarda o tempo da morte na sessão atual
    public void SetTimeOfDeath()
    {
        if (currentSession == null)
        {
            LogToFile("currentSession is null in SetTimeOfDeath!");
            return;
        }

        LogToFile($"Setting timeOfDeath..");
        currentSession.timeOfDeath = GetElapsedTime();
    }

    // Retorna o tempo passado desde o início da contagem
    public float GetElapsedTime()
    {
        if (stopwatch == null)
        {
            LogToFile("Stopwatch not started!");
            return 0f;
        }
        return (float)stopwatch.Elapsed.TotalSeconds;
    }

    // Inicia a contagem do tempo
    public void SetElapsedTime()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();
        LogToFile("Stopwatch started.");
    }

    // Começa uma nova sessão para um nível
    public void StartNewSession(GameObject level)
    {
        if (currentSession != null)
        {
            LogToFile("A session is already active. Ending it before starting a new one.");
            EndCurrentSession();
        }

        string playerName = PlayerPrefs.GetString("PlayerName", "Player");
        currentPlayer = playerList.players.Find(p => p.name == playerName);

        if (currentPlayer == null)
        {
            currentPlayer = new PlayerData { name = playerName };
            playerList.players.Add(currentPlayer);
        }

        // por enquanto, não adiciona plataformas, deixa lista vazia
        currentSession = new SessionData
        {
            level = level.name,
            startTime = DateTime.Now.ToString(),
            obstacles = new List<ObstacleData>()
        };

        LogToFile($"New session started for player: {playerName}");
    }

    public void EndCurrentSession()
    {
        if (currentSession == null || currentPlayer == null)
        {
            LogToFile("No active session to end.");
            return;
        }

        currentSession.endTime = DateTime.Now.ToString();

        currentPlayer.sessions.Add(currentSession);

        SavePlayersToJson();

        currentSession = null;
        obstacles.Clear();
        obstacleId = 0;
        gameStartTime = 0f;
        pendingTaps.Clear();

        LogToFile("Session ended successfully.");
    }

    private void SavePlayersToJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "HopfuryPlayersData.json");
        string json = JsonUtility.ToJson(playerList, true);
        File.WriteAllText(path, json);

        LogToFile("JSON saved at: " + path);
    }

    private void LoadPlayersFromJson()
    {
        string path = Path.Combine(Application.persistentDataPath, "HopfuryPlayersData.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            playerList = JsonUtility.FromJson<PlayerList>(json);
            LogToFile("JSON loaded from: " + path);
        }
        else
        {
            LogToFile("No JSON file found, creating new player list.");
            playerList = new PlayerList();
        }
    }

    // Método para gravar informações no arquivo de log
    public void LogToFile(string message)
    {
        try
        {
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Dispose(); // Cria e fecha imediatamente
            }

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
        catch (Exception e)
        {
            LogToFile($"Failed to write to log file: {e.Message}");
        }
    }

}
