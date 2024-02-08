using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUIElements : MonoBehaviour
{
    public Image FSBar;
    public GameObject[] playerStocks;
    public TextMeshProUGUI damageText, playerNameText;
    public Image imgIcon;
    private void Awake()
    {
        FSBar.fillAmount = 0;
        foreach (GameObject playerStock in playerStocks) playerStock.SetActive(true);
        damageText.text = "0%";
    }
}