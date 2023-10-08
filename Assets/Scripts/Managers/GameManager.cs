using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Clase para manejar variables estaticas / globales
public class GameManager : MonoBehaviour
{
    [Header ("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText;
    [Header ("Static Constants")]
    public static bool usingEditor;
    public static GameManager instance;
    public static List<CharacterStats> players = new ();
    private void Awake()
    {
        instance = this;
        usingEditor = false;
        #if UNITY_EDITOR
        usingEditor = true;
        #endif
    }
    public static void EnablePlayers() { foreach (CharacterStats player in players) player.GetComponent<ComponentsManager>().inputManagement.enabled = true; }
    public void GameOver(CharacterStats winner = null)
    {
        if (usingEditor) Debug.Log("Game over! " + (winner != null ? winner : ""));
        if(gameOverPanel != null) gameOverPanel.SetActive(true);
        if(winner != null && winnerText != null) winnerText.text = (string.IsNullOrEmpty(winner.playerName) ? "Player" + winner.playerNumber : winner.playerName) + " won!";
    }
    
    public static List<GameObject> FindPlayers()
    {
        List<GameObject> _players = new();
        foreach (CharacterStats playerStats in players) _players.Add(playerStats.gameObject);
        if(_players.Count < 2) foreach (CharacterStats playerStats in FindObjectsOfType<CharacterStats>()) _players.Add(playerStats.gameObject);
        return _players;
    }
}