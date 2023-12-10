using System.Collections;
using Cinemachine;
using CustomClasses;
using Online;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CameraManager : InstanceClass<CameraManager>
{
    public bool trainingRoom = false;
    public CinemachineVirtualCamera vCamSelection, vCamGameplay;
    public Camera mainCamera, platformCamera, stageCamera;

    void Start()
    {
        if(!trainingRoom)
        {
            CameraSwitcher.instance.Register(vCamGameplay);
            CameraSwitcher.instance.SwitchCamera(vCamSelection);
            mainCamera.enabled = platformCamera.enabled = stageCamera.enabled = false;
        }
        else SetGameplayView();
    }
    public void SetGameplayView() => StartCoroutine(SetGameplayViewRoutine());
    private IEnumerator SetGameplayViewRoutine()
    {
        SpawnPlayers.instance.enabled = true;
        if(!trainingRoom)
        {
            CameraSwitcher.instance.SwitchCamera(vCamGameplay);
            if(GameplayManager.instance.multipleUISelection != null) 
                GameplayManager.instance.multipleUISelection.OnlyShowElements("GameplayPanel");
            yield return new WaitForSeconds(2f);
            mainCamera.enabled = platformCamera.enabled = stageCamera.enabled = true;
        }
        GameplayManager.instance.canStart = true;
    }
    public void SetCharacterSelectionView() => StartCoroutine(SetCharacterSelectionViewRoutine());
    private IEnumerator SetCharacterSelectionViewRoutine()
    {
        RoomManager.instance.SendPlayerProperty(new Hashtable(){{ConnectToServer.READY, false}});
        foreach (TextMeshProUGUI readyText in RoomManager.instance.readyTexts) if(readyText) readyText.gameObject.SetActive(false);
        GameplayManager.instance.winnerText.text = "";
        SpawnPlayers.instance.enabled = false;
        if(!trainingRoom)
        {
            mainCamera.enabled = platformCamera.enabled = stageCamera.enabled = false;
            yield return new WaitForSeconds(2f);
            CameraSwitcher.instance.SwitchCamera(vCamSelection);
            if(GameplayManager.instance.multipleUISelection != null)
                GameplayManager.instance.multipleUISelection.OnlyShowElements("CharacterSelectionPanel", "PlayerPanels");
        }
        GameplayManager.instance.canStart = false;
    }
}