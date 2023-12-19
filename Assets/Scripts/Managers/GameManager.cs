using System.Collections.Generic;
using System.Linq;
using CustomClasses;
using Photon.Pun;
using UnityEngine;

// Clase para manejar variables estaticas / globales
public class GameManager : InstanceClass<GameManager>
{
    [Header ("Static Constants")]
    public static bool usingEditor;
    public static int currentPlayer;
    public static List<CharacterStats> players = new ();
    protected override void Awake()
    {
        base.Awake();        
        usingEditor = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;
    }

    public static void RegisterPlayer(CharacterStats player)
    {
        players.Add(player);
        MatchData.instance.UpdatePlayersData(player);
        if(PhotonNetwork.IsConnected && players.Count == PhotonNetwork.PlayerList.Length)
        {
            List<CharacterStats> _players = players.OrderBy(player => player.playerNumber).ToList();
            players = _players;
            GameplayManager.instance.StartGame();
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