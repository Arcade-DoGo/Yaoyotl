using System.Collections;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Online;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

public class MatchData : MonoBehaviour
{
    public static MatchData instance;
    public TextMeshProUGUI[] stocks;
    public TextMeshProUGUI[] playerDamages;
    public TextMeshProUGUI timer;
    private int minutes;
    private int seconds;

    private void Awake() => instance = this;
    public void updatePlayersData(CharacterStats stats)
    {
        int playerIndex = GameManager.players.IndexOf(stats);
        stocks[playerIndex].SetText(stats.stocks + "");
        playerDamages[playerIndex].SetText(stats.damage + "%");
        // if(PhotonNetwork.IsConnected)
        // {
        //     PhotonView photonView = stats.GetComponent<ComponentsManager>().photonView;
        //     photonView.RPC("UpdatePlayersDataRPC", RpcTarget.Others, photonView.Owner, ConnectToServer.STOCKS, stats.stocks);
        //     photonView.RPC("UpdatePlayersDataRPC", RpcTarget.Others, photonView.Owner, ConnectToServer.DAMAGE, stats.damage);
        //     int playerIndex = GameManager.players.IndexOf(stats);
        //     stocks[playerIndex].SetText(stats.stocks + "");
        //     playerDamages[playerIndex].SetText(stats.damage + "%");
        // }
        // else
        // {
        //     int playerIndex = GameManager.players.IndexOf(stats);
        //     stocks[playerIndex].SetText(stats.stocks + "");
        //     playerDamages[playerIndex].SetText(stats.damage + "%");
        // }
    }

    public void updatePlayersData(int playerNumber, int playerStocks, float playerDamage)
    {
        int playerIndex = GameManager.players.IndexOf(GameManager.players[playerNumber]);
        stocks[playerIndex].SetText(playerStocks + "");
        playerDamages[playerIndex].SetText(playerDamage + "%");
        // if(PhotonNetwork.IsConnected)
        // {
        //     PhotonView photonView = GameManager.players[playerNumber].GetComponent<ComponentsManager>().photonView;
        //     photonView.RPC("UpdatePlayersDataRPC", RpcTarget.Others, photonView.Owner, ConnectToServer.STOCKS, playerStocks);
        //     photonView.RPC("UpdatePlayersDataRPC", RpcTarget.Others, photonView.Owner, ConnectToServer.DAMAGE, playerDamage);
        //     int playerIndex = photonView.OwnerActorNr;
        //     stocks[playerIndex].SetText(playerStocks + "");
        //     playerDamages[playerIndex].SetText(playerDamage + "%");
        // }
        // else
        // {
        //     int playerIndex = GameManager.players.IndexOf(GameManager.players[playerNumber]);
        //     stocks[playerIndex].SetText(playerStocks + "");
        //     playerDamages[playerIndex].SetText(playerDamage + "%");
        // }
    }

    [PunRPC]
    private void UpdatePlayersDataRPC(Player player, string property, int value)
    {
        if (!player.CustomProperties.ContainsKey(property)) player.CustomProperties.Add(property, value);
        player.CustomProperties[property] = value;
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { property, value } });
    }

    public void StartTimer() => StartCoroutine(updateTimer());
    private IEnumerator updateTimer()
    {
        minutes = 3;
        seconds = 0;
        string timerText;
        while (minutes > 0 || seconds > 0)
        {
            seconds--;
            if (seconds < 0)
            {
                seconds = 59;
                minutes--;
            }

            timerText = minutes + ":" + (seconds <= 9 ? "0" + seconds : seconds);
            timer.SetText(timerText);
            yield return new WaitForSeconds(1f);
        }

        // Game Over: No winner
        GameManager.instance.GameOver();
    }
}
