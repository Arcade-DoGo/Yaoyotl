using System.Collections.Generic;
using System.Linq;
using Online;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Clase para manejar variables estaticas / globales
public class GameManager : MonoBehaviour
{
    [Header ("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI winnerText, player1Text, player2Text;
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
        if(gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public static void RegisterPlayer(CharacterStats player)
    {
        players.Add(player);
        if(PhotonNetwork.IsConnected && players.Count == PhotonNetwork.PlayerList.Length)
        {
            List<CharacterStats> _players = players.OrderBy(player => player.playerNumber).ToList();
            players = _players;
            SpawnPlayers.instance.StartGame();
        }
    }

    public static void EnablePlayers() { foreach (CharacterStats player in players) player.GetComponent<ComponentsManager>().inputManagement.enabled = true; }
    public void GameOver(CharacterStats winner = null)
    {
        foreach (CharacterStats player in players)
        {
            player.GetComponent<CharacterMovement>().enabled = false;
            player.GetComponent<Attack>().enabled = false;
        }
        if (usingEditor) Debug.Log("Game over! " + (winner != null ? winner : ""));
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            RoomManager.instance.Init();
        }
        if(player1Text != null) player1Text.text = players[0].playerName;
        if(player2Text != null) player2Text.text = players[1].playerName;
        if(winner != null && winnerText != null)
        {
            // players = new List<CharacterStats>(){winner};
            winnerText.text = (string.IsNullOrEmpty(winner.playerName) ? "Player" + winner.playerNumber : winner.playerName) + " won!";
        }
    }
    
    public static List<GameObject> FindPlayers()
    {
        List<GameObject> _players = new();
        foreach (CharacterStats playerStats in players) _players.Add(playerStats.gameObject);
        if(_players.Count < 2) foreach (CharacterStats playerStats in FindObjectsOfType<CharacterStats>()) _players.Add(playerStats.gameObject);
        return _players;
    }
}