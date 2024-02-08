using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [Header ("Player Status")]
    [NonSerialized] public bool onLedge, isGrounded, isDoubleJumping, canFastFall, isAttacking, isGettingUp, inHitStun, canFAM;
    [NonSerialized] public bool isFacingRight = true;

    [Header ("UI Components")]
    [NonSerialized] public PlayerStatsUIElements playerStatsUI;

    [Header ("Player Info")]
    public string playerName;
    public int playerNumber = 1;

    [Header ("Player General data")]
    public float damage = 0.0f;
    public float weight = 1.0f;
    public int stocks = 3;

    [Header ("Player movement")]
    public float groundSpeed = 10.0f;

    [Header ("Player jumping")]
    [Tooltip("Horizontal speed in the air")]
    public float airSpeed = 7.5f;
    [Tooltip("Falling speed of the player when free falling")]
    public float fallMultiplier = 0.5f;
    [Tooltip("Speed modifier for short jump")]
    public float lowJumpMultiplier = 1.5f;
    [Tooltip("Initial jump force")]
    public float jumpForce = 13.0f;
    [Tooltip("Fast fall force")]
    public float fallForce = 10f;
    [Tooltip("Max jumps that can do the player in total")]
    public int maxJumps = 2;

    [Header ("Private variables")]
    private float FAM = 0f;
    private bool onlinePlayer = false;
    private readonly float fullFAM = 100f;
    [NonSerialized] public bool NPC = false;
    [NonSerialized] public int jumpsUsed = 0;
    private readonly int secondsTillFAM = 10; // 2.5 minutes

    [Header ("Private components")]
    private CharacterController controller;
    private PhotonView photonView;
    private ComponentsManager cm;

    private void Awake()
    {
        cm = GetComponent<ComponentsManager>();
        onlinePlayer = PhotonNetwork.IsConnected && !name.Contains("Offline");
        if (onlinePlayer)
        {
            photonView = cm.photonView;
            SetPlayerInfo(photonView.Owner.NickName, photonView.Owner.ActorNumber);
            // if (GameManager.usingEditor) Debug.Log("Added " + playerName + " in " + GameManager.players.Count);
        }
    }

    void Start()
    {
        controller = cm.characterController;
        GameManager.RegisterPlayer(this);
        StartCoroutine(chargeFAM());
    }

    public void SetPlayerInfo(string _name, int _number)
    {
        playerName = _name;
        playerStatsUI.playerNameText.text = _name;
        playerNumber = _number;
        cm.playerUI.playerNumber.text = "P" + (_number + 1);
    }

    public void addDamage(float _damage)
    {
        if(onlinePlayer)
        {
            if(photonView.IsMine)
            {
                damage += _damage;
                increaseFAM(damage / 5f);
                controller.SyncPlayerData(false);
            }

        }
        else
        {
            damage += _damage;
            increaseFAM(damage / 5f);
            MatchData.instance.UpdatePlayersData(this);
        }
    }

    public void loseStock()
    {
        if(onlinePlayer)
        {
            if(photonView.IsMine)
            {
                stocks--;
                damage = 0;
                FAM /= 2f;
                controller.SyncPlayerData(true);
            }
        }
        else
        {
            stocks--;
            damage = 0;
            FAM /= 2f;
            MatchData.instance.UpdatePlayersData(this);
            controller.Respawn();
        }
        playerStatsUI.playerStocks[stocks].SetActive(false);
    }

    public void resetFAM()
    {
        FAM = 0f;
        canFAM = false;
        StartCoroutine(chargeFAM());
    }

    void increaseFAM(float amount)
    {
        float newFAM = FAM + amount;
        FAM = newFAM < fullFAM ? newFAM : fullFAM; // Keep meter under maximum
        playerStatsUI.FSBar.fillAmount = (FAM / 100);
    }

    IEnumerator chargeFAM()
    {
        while (!canFAM)
        {
            float timeStep = 0.1f;
            float meterStep = timeStep * fullFAM / secondsTillFAM;
            yield return new WaitForSeconds(timeStep);
            increaseFAM(meterStep);
            canFAM = FAM >= fullFAM; // is meter full?
        }
        print(FAM + "/100");
    }
}