using System.Collections;
using UnityEngine;
using TMPro;

public class MatchData : MonoBehaviour
{
    public static MatchData instance;
    public TextMeshProUGUI[] stocks, playerDamages;
    public TextMeshProUGUI timer;
    private int minutes, seconds;
    private void Awake() => instance = this;
    public void UpdatePlayersData(CharacterStats stats)
    {
        // Debug.Log("PROPERTIES UPDATE: " + stats.playerName + " " + stats.stocks + " " + stats.damage);
        int playerIndex = GameManager.players.IndexOf(stats);
        stocks[playerIndex].text = stats.stocks + "";
        playerDamages[playerIndex].text = stats.damage + "%";
        if(stats.stocks == 0) GameplayManager.instance.GameOver(GameManager.players.Find(player => player != stats));
    }
    public void StartTimer() => StartCoroutine(UpdateTimer());
    private IEnumerator UpdateTimer()
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
            timer.text = timerText;
            yield return new WaitForSeconds(1f);
        }

        // Game Over: No winner
        GameplayManager.instance.GameOver();
    }
}