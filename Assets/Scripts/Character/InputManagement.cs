using UnityEngine;

public class InputManagement : MonoBehaviour
{
    public FixedJoystick joystick;
    public GameObject mobileInputCanvas;

    public static float horizontal;
    public static bool jumpInput, jumpRelease, crouchInput;

    private bool _jumpUI, _crouchUI, _jumpReleaseUI;
    private bool mobileDevice, usingEditor;

    private void Start() 
    {
        #if UNITY_EDITOR
        usingEditor = true;
        #else
        usingEditor = false;
        #endif

        _jumpUI = false;
        _crouchUI = false;
        _jumpReleaseUI = false;
        mobileDevice = Application.platform == RuntimePlatform.Android;
        mobileInputCanvas.SetActive(mobileDevice || usingEditor);
    }
    private void Update() 
    {
        horizontal = Input.GetAxis("Horizontal") + joystick.Horizontal;
        jumpInput = Input.GetButtonDown("Jump") || _jumpUI; // Press Jump (Up)
        jumpRelease = Input.GetButtonUp("Jump") || _jumpReleaseUI; // Release Jump (Up)
        crouchInput = Input.GetButtonDown("Crouch") || _crouchUI; // Press Crouch (Down)

        _jumpUI = _crouchUI = _jumpReleaseUI = false;
    }
    public void JumpButton(bool value) { _jumpUI = value; _jumpReleaseUI = !value; }
    public void CrouchButton(bool value) => _crouchUI = value;
}