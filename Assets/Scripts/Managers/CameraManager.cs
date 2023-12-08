using System.Collections;
using Cinemachine;
using Online;
using UnityEngine;

public class CameraManager : MonoBehaviour
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
        if(GameplayManager.instance.multipleUISelection != null) 
            GameplayManager.instance.multipleUISelection.OnlyShowElements("GameplayPanel");
        if(!trainingRoom)
        {
            CameraSwitcher.instance.SwitchCamera(vCamGameplay);
            yield return new WaitForSeconds(2f);
            mainCamera.enabled = platformCamera.enabled = stageCamera.enabled = true;
        }
        GameplayManager.instance.canStart = true;
    }
}