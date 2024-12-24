using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ScoreManager2 : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnScore1Changed))]
    private int score1;

    [SyncVar(hook = nameof(OnScore2Changed))]
    private int score2;

    public int maxScore = 3;
    public Text scoreText1;
    public Text scoreText2;

    public CanvasManager canvasManager;

    // Objeto que se activará al ganar
    public GameObject victoryObject;

    void Start()
    {
        if (canvasManager == null)
            canvasManager = FindObjectOfType<CanvasManager>();

        if (victoryObject != null)
        {
            victoryObject.SetActive(false);
        }
    }

    void Update()
    {
        CheckVictoryObjectVisibility();
    }

    public void UpdateScore(int playerIndex)
    {
        if (!isServer) return;

        if (playerIndex == 1)
        {
            score1++;
        }
        else if (playerIndex == 2)
        {
            score2++;
        }

        CheckVictoryObjectVisibility(); // Verifica si debe ocultar el objeto
    }

    private void OnScore1Changed(int oldScore, int newScore)
    {
        scoreText1.text = "Player1: " + newScore;
        CheckForVictory(newScore, 1);
    }

    private void OnScore2Changed(int oldScore, int newScore)
    {
        scoreText2.text = "Player2: " + newScore;
        CheckForVictory(newScore, 2);
    }

    private void CheckForVictory(int score, int playerNumber)
    {
        if (score >= maxScore)
        {
            Debug.Log($"¡Player{playerNumber} ha ganado!");

            if (victoryObject != null)
            {
                victoryObject.SetActive(true);
            }

            if (canvasManager != null)
            {
                if (UnityEngine.XR.XRSettings.isDeviceActive)
                {
                    canvasManager.ShowVrCanvas();
                }
                else
                {
                    canvasManager.ShowNonVrCanvas();
                }
            }
        }
    }

    public void ResetScores()
    {
        if (isServer)
        {
            score1 = 0;
            score2 = 0;

            CheckVictoryObjectVisibility(); // Verifica si debe ocultar el objeto
        }
    }

    public void EndGame()
    {
        if (isServer)
        {
            ResetScores(); // Reinicia los puntajes
            Debug.Log("Cerrando la partida...");
            NetworkManager.singleton.StopHost();
        }
        else
        {
            Debug.LogWarning("Solo el servidor puede cerrar la partida.");
        }
    }

    private void CheckVictoryObjectVisibility()
    {
        // Oculta el objeto si ambos puntajes son 0
        if (victoryObject != null && score1 == 0 && score2 == 0)
        {
            victoryObject.SetActive(false);
        }
    }
}
