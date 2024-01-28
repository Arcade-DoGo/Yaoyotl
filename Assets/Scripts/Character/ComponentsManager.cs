using Photon.Pun;
using TMPro;
using UnityEngine;

// This script helps provide a component center where you can put any script publicly and get its reference in here
public class ComponentsManager : MonoBehaviour
{
    [Header ("Component References")]
    public InputManagement inputManagement;
    public CharacterStats characterStats;
    public CharacterController characterController;
    public new Rigidbody rigidbody;
    public PhotonAnimatorView photonAnimatorView;
    public PhotonView photonView;
    public Animator animator;
    public TextMeshProUGUI playerNumber;
    public CharacterAnimate charAnim;
    public FinalSmash finalSmash;
}
