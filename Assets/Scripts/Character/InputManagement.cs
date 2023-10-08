using Photon.Pun;
using UnityEngine;

public class InputManagement : MonoBehaviour
{
    [Header ("UI References")]
    public ComponentsManager componentsManager;
    public FixedJoystick joystick;
    public GameObject mobileInputCanvas, jumpTapButtons;
    #region InputVariables
    public static float horizontal;
    public static bool mobileDevice, jumpTap,
        jumpInput, jumpRelease, jumpHold, crouchInput, crouchHold, 
        attackInput, attackRelease, finalAttackInput;
    private float joystickDownInput, joystickUpInput;
    private bool _jumpPressUI, _jumpReleaseUI, _jumpHoldUI, _crouchPressUI, _crouchHoldUI, 
                _attackInputUI, _attackReleaseUI, _finalAttackInputUI;
    #endregion

    private void Start()
    {
        _jumpPressUI = _jumpReleaseUI = _jumpHoldUI = _crouchPressUI = _crouchHoldUI = 
        _attackInputUI = _attackReleaseUI = _finalAttackInputUI = false;
        mobileDevice = Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.OSXPlayer;
        mobileInputCanvas.SetActive((mobileDevice || GameManager.usingEditor) && componentsManager.photonView.IsMine);
        jumpTapButtons.SetActive(jumpTap || GameManager.usingEditor);
    }
    private void Update()
    {
        if(componentsManager.photonView.IsMine)
        {
            #region JoystickInputs
            horizontal = Input.GetAxis("Horizontal") + joystick.Horizontal;
            joystickDownInput = Mathf.Clamp(joystick.Vertical, -1f, -0.01f);
            joystickUpInput = Mathf.Clamp(joystick.Vertical, 0.01f, 1f);
            #endregion

            #region Jump
            jumpInput = Input.GetButtonDown("Jump") 
                        || (_jumpPressUI && jumpTap)
                        || (joystick.Pressed && joystickUpInput > 0f && !jumpTap); // Press Jump (Up)
            jumpHold = Input.GetButton("Jump") 
                        || (_jumpHoldUI && jumpTap)
                        || (joystick.Held && joystickUpInput > 0f && !jumpTap); // Hold Jump (Up)
            jumpRelease = Input.GetButtonUp("Jump") 
                        || (_jumpReleaseUI && jumpTap)
                        || (joystick.Released && joystickUpInput > 0f && !jumpTap); // Release Jump (Up)
            #endregion

            #region Crouch
            crouchInput = Input.GetButtonDown("Crouch") 
                        || (_crouchPressUI && jumpTap) 
                        || (joystick.Pressed && joystickDownInput < 0f && !jumpTap); // Press Crouch (Down)
            crouchHold = Input.GetButton("Crouch") 
                        || (_crouchHoldUI && jumpTap)
                        || (joystick.Held && joystickDownInput < 0f && !jumpTap); // Hold Crouch (Down) 
            #endregion

            #region Attack
            attackInput = Input.GetButtonDown("Attack") || _attackInputUI; // Press Attack (Left Click)
            attackRelease = Input.GetButtonUp("Attack") || _attackReleaseUI; // Release Attack (Left Click)
            finalAttackInput = Input.GetButtonDown("FinalAttack") || _finalAttackInputUI; // Press Final Attack (Right Click)
            #endregion

            #region EndFrameInputs
            _jumpPressUI = _jumpReleaseUI = _crouchPressUI = 
            _attackInputUI = _attackReleaseUI = _finalAttackInputUI = false;
            #endregion
        }
    }
    #region InputButtons
    public void JumpButton(bool value) { _jumpPressUI = value; _jumpReleaseUI = !value; _jumpHoldUI = value; }
    public void CrouchButton(bool value) { _crouchPressUI = value; _crouchHoldUI = value; }
    public void AttackButton(bool value) { _attackInputUI = value; _attackReleaseUI = !value; }
    public void FinalAttackButton(bool value) { _finalAttackInputUI = value; }
    #endregion
}