using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class MatchData : MonoBehaviour
{
    public static MatchData instance;
    public TextMeshProUGUI timer;
    public float gameplayTime;
    private int minutes, seconds;
    [NonSerialized] public List<PlayerStatsUIElements> playerStatsUI;
    private void Awake() { instance = this; playerStatsUI = new(); }
    public void UpdatePlayersData(CharacterStats stats)
    {
        // Debug.Log("PROPERTIES UPDATE: " + stats.playerName + " " + stats.stocks + " " + stats.damage);
        int playerIndex = GameManager.players.IndexOf(stats);
        playerStatsUI[playerIndex].damageText.text = stats.damage + "%";

        for (int stock = 0; stock < stats.stocks; stock++)
            playerStatsUI[playerIndex].playerStocks[stock].SetActive(true);

        if(stats.stocks == 0) GameplayManager.instance.GameOver(GameManager.players.Find(player => player != stats));
    }
    public void StartTimer() => StartCoroutine(UpdateTimer());
    private IEnumerator UpdateTimer()
    {
        minutes = Mathf.RoundToInt(gameplayTime / 60);
        seconds = (int) gameplayTime % minutes;
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