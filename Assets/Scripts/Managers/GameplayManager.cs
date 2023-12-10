using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using CustomClasses;
using Online;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameplayManager : InstanceClass<GameplayManager>
{
    [Header ("UI References")]
    public MultipleUISelection multipleUISelection;
    public TextMeshProUGUI winnerText, startText;
    [NonSerialized] public bool canStart;

    private void OnEnable() 
    {
        if(multipleUISelection != null) multipleUISelection.OnlyShowElements("CharacterSelectionPanel", "PlayerPanels");
    }

    public void StartGame() => StartCoroutine(StartGameRoutine());
    private IEnumerator StartGameRoutine()
    {
        yield return new WaitUntil(() => canStart);
        if(PhotonNetwork.IsConnected)
        {
            if(startText) startText.text = "";
            yield return new WaitForSeconds(0.5f);
            for (int i = 3; i > 0; i--)
            {
                if(startText) startText.text = i + "";
                yield return new WaitForSeconds(1f);
            }
        }
        if(startText) startText.text = "GO!";
        EnablePlayers();
        MatchData.instance.StartTimer();
        yield return new WaitForSeconds(1f);
        if(startText) startText.text = "";
    }

    public static void EnablePlayers() { foreach (CharacterStats player in GameManager.players) player.GetComponent<ComponentsManager>().inputManagement.enabled = true; }
    public void GameOver(CharacterStats winner = null)
    {
        if(multipleUISelection != null)
        {
            multipleUISelection.OnlyShowElements("GameOverPanel", "PlayerPanels");
            RoomManager.instance.ShowRoom();
        }

        RoomManager.instance.SendPlayerProperty(new Hashtable(){{ConnectToServer.READY, false}});
        RoomManager.instance.SetPlayerText(0, GameManager.players[0].playerName);
        RoomManager.instance.SetPlayerText(1, GameManager.players[1].playerName);
        if(winnerText != null)
            winnerText.text = winner != null ? (string.IsNullOrEmpty(winner.playerName) ? 
            "Player" + winner.playerNumber : winner.playerName) + " won!" : "Tie!";
        
        int players = GameManager.players.Count;
        for (int i = 0; i < players; i++) Destroy(GameManager.players[i].gameObject);
        GameManager.players.Clear();
    }
}