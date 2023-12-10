using System.Collections.Generic;
using Cinemachine;
using CustomClasses;

public class CameraSwitcher : InstanceClass<CameraSwitcher>
{
    private CinemachineVirtualCamera activeCamera = null;
    private readonly List<CinemachineVirtualCamera> cameras = new();
    
    #region Public Methods
    // This method adds a camera to the static list of cameras that can be switched around.
    public void Register(params CinemachineVirtualCamera[] _cameras) { foreach(CinemachineVirtualCamera camera in _cameras) cameras.Add(camera); }
    // This method Returns if a camera is the current activeCamera.
    public bool IsActiveCamera(CinemachineVirtualCamera camera) => camera == activeCamera;
    // This method Returns the current activeCamera.
    public CinemachineVirtualCamera GetActiveCamera() => activeCamera;
    // This method recives a CinemachineVirtualCamera and sets it as the current activeCamera.
    public void SwitchCamera(CinemachineVirtualCamera currentCamera)
    {
        if(!cameras.Contains(currentCamera)) Register(currentCamera);

        foreach (CinemachineVirtualCamera camera in cameras) camera.Priority = 0;
        activeCamera = currentCamera;
        activeCamera.Priority = 10;
    }
    #endregion
}