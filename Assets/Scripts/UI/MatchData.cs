using System.Collections;
using UnityEngine;
using TMPro;

public class MatchData : MonoBehaviour
{
    public static MatchData instance;
    public TextMeshProUGUI[] stocks;
    public TextMeshProUGUI[] playerDamages;
    public TextMeshProUGUI timer;
    private int minutes;
    private int seconds;

    private void Awake() => instance = this;
    void Start()
    {
        minutes = 3;
        seconds = 0;
        string timerText = minutes + ":" + (seconds <= 9 ? "0" + seconds : seconds);
        timer.SetText(timerText);

    }

    public void updatePlayersData(CharacterStats stats)
    {
        int playerIndex = GameManager.players.IndexOf(stats);
        stocks[playerIndex].SetText(stats.stocks + "");
        playerDamages[playerIndex].SetText(stats.damage + "%");
    }

    public void updatePlayersData(int playerNumber, int playerStocks, float playerDamage)
    {
        int playerIndex = GameManager.players.IndexOf(GameManager.players[playerNumber]);
        stocks[playerIndex].SetText(playerStocks + "");
        playerDamages[playerIndex].SetText(playerDamage + "%");
    }

    public void StartTimer() => StartCoroutine(updateTimer());
    private IEnumerator updateTimer()
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

        // Game Over: No winner
        GameManager.instance.GameOver();
    }
}
