using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchData : MonoBehaviour
{
    public TextMeshProUGUI[] stocks;
    public TextMeshProUGUI[] playerDamages;
    public TextMeshProUGUI timer;

    public void updatePlayersData(CharacterStats stats)
    {
        int playerIndex = stats.playerNumber - 1;

        stocks[playerIndex].SetText(stats.stocks + "");
        playerDamages[playerIndex].SetText(stats.damage + "%");
    }

    public void updatePlayersData(int playerNumber, int playerStocks, float playerDamage)
    {
        int playerIndex = playerNumber - 1;

        stocks[playerIndex].SetText(playerStocks + "");
        playerDamages[playerIndex].SetText(playerDamage + "%");
    }
}
