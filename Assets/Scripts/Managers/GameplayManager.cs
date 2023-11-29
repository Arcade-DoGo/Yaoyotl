using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;
using CustomClasses;
using Online;
using Cinemachine;

public class GameplayManager : InstanceClass<GameplayManager>
{
    public bool trainingRoom = false;
    [Header ("UI References")]
    public GameObject gameOverPanel;
    public MultipleUISelection multipleUISelection;
    public CinemachineVirtualCamera vCamSelection, vCamGameplay;
    public TextMeshProUGUI winnerText, startText;
    public Camera mainCamera, platformCamera;
    private bool canStart;

    private void Start() 
    {
        if(gameOverPanel != null) gameOverPanel.SetActive(false);
        if(multipleUISelection != null) multipleUISelection.OnlyShowElements("CharacterSelectionPanel", "PlayerPanels");
        if(!trainingRoom)
        {
            CameraSwitcher.instance.Register(vCamSelection);
            CameraSwitcher.instance.Register(vCamGameplay);
            CameraSwitcher.instance.SwitchCamera(vCamSelection);
            mainCamera.enabled = false;
            platformCamera.enabled = false;
        }
        else SetGameplayView();
    }
    
    public void SetGameplayView() => StartCoroutine(SetGameplayViewRoutine());
    private IEnumerator SetGameplayViewRoutine()
    {
        SpawnPlayers.instance.enabled = true;
        if(multipleUISelection != null) multipleUISelection.OnlyShowElements("GameplayPanel");
        if(!trainingRoom)
        {
            CameraSwitcher.instance.SwitchCamera(vCamGameplay);
            yield return new WaitForSeconds(2f);
            mainCamera.enabled = true;
            platformCamera.enabled = true;
        }
        canStart = true;
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
        foreach (CharacterStats player in GameManager.players)
        {
            player.GetComponent<CharacterMovement>().enabled = false;
            player.GetComponent<Attack>().enabled = false;
        }
        if (GameManager.usingEditor) Debug.Log("Game over! " + (winner != null ? winner : ""));
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            RoomManager.instance.Init();
        }
        RoomManager.instance.SetPlayerText(0, GameManager.players[0].playerName);
        RoomManager.instance.SetPlayerText(1, GameManager.players[1].playerName);
        if(winner != null && winnerText != null) winnerText.text = (string.IsNullOrEmpty(winner.playerName) ? "Player" + winner.playerNumber : winner.playerName) + " won!";
    }
}