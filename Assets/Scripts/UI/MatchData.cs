using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MatchData : MonoBehaviour
{
    public TextMeshProUGUI[] stocks;
    public TextMeshProUGUI[] playerDamages;
    public TextMeshProUGUI timer;
    private int minutes;
    private int seconds;

    void Start()
    {
        minutes = 3;
        seconds = 0;
        string timerText = minutes + ":" + (seconds <= 9 ? "0" + seconds : seconds);
        timer.SetText(timerText);

        StartCoroutine(updateTimer());
    }

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

    IEnumerator updateTimer()
    {
        while (minutes > 0 || seconds > 0)
        {
            yield return new WaitForSeconds(1f);

            seconds--;
            if (seconds < 0)
            {
                seconds = 59;
                minutes--;
            }

            string timerText = minutes + ":" + (seconds <= 9 ? "0" + seconds : seconds);

            timer.SetText(timerText);
        }

        // Game Over
    }
}
