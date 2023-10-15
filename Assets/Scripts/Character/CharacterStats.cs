using System;
using System.Collections;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine;
using Online;

public class CharacterStats : MonoBehaviour
{
    [NonSerialized] public bool onLedge, isGrounded, isDoubleJumping, canFastFall, isAttacking, isGettingUp, inHitStun, canFAM;
    [NonSerialized] public bool isFacingRight = true;
    public string playerName;
    public int playerNumber = 1;
    public float damage = 0.0f;
    public float weight = 1.0f;
    public float groundSpeed = 10.0f;
    public float airSpeed = 7.5f;
    public float jumpForce = 13.0f;
    public float shortJumpForce = 10.0f;
    public float fallForce = 10f;
    public int maxJumps = 2;
    public int jumpsUsed = 0;
    public int stocks = 3;

    private float FAM = 0f;
    private float fullFAM = 100f;
    private int secondsTillFAM = 150; // 2.5 minutes

    private Animator animator;
    private PhotonView photonView;

    private void Awake()
    {
        if (PhotonNetwork.IsConnected)
        {
            photonView = GetComponent<ComponentsManager>().photonView;
            playerName = photonView.Owner.NickName;
            playerNumber = photonView.Owner.ActorNumber;
            GameManager.RegisterPlayer(this);
            if (GameManager.usingEditor) Debug.Log("Added " + playerName + " in " + GameManager.players.Count);
        }
    }
    void Start()
    {
        animator = GetComponent<ComponentsManager>().animator;
        MatchData.instance.updatePlayersData(this);

        StartCoroutine(chargeFAM());
    }

    public void addDamage(float damage)
    {
        this.damage += damage;
        increaseFAM(damage / 5f);
        print("FAM Damage Increase--------------------");
        if(PhotonNetwork.IsConnected)
        {
            if(photonView.IsMine)
                photonView.RPC("SyncPlayerData", RpcTarget.All, ConnectToServer.DAMAGE, damage);
        }
        else
            MatchData.instance.updatePlayersData(this);
    }

    public void loseStock()
    {
        stocks--;
        damage = 0;
        FAM /= 2f;
        if(PhotonNetwork.IsConnected)
        {
            if(photonView.IsMine)
                photonView.RPC("SyncPlayerData", RpcTarget.All, ConnectToServer.STOCKS, stocks);
        }
        else
            MatchData.instance.updatePlayersData(this);
    }
    
    [PunRPC]
    private void SyncPlayerData(string property, int value)
    {
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(property)) PhotonNetwork.LocalPlayer.CustomProperties.Add(property, value);
        PhotonNetwork.LocalPlayer.CustomProperties[property] = value;
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { property, value } });
    }

    public void resetFAM()
    {
        FAM = 0f;
        setCanFAM(false);
        StartCoroutine(chargeFAM());
    }

    void increaseFAM(float amount)
    {
        float newFAM = FAM + amount;
        FAM = newFAM < fullFAM ? newFAM : fullFAM; // Keep meter under maximum
    }

    IEnumerator chargeFAM()
    {
        while (!canFAM)
        {
            float timeStep = 0.1f;
            float meterStep = timeStep * fullFAM / secondsTillFAM;
            yield return new WaitForSeconds(timeStep);
            increaseFAM(meterStep);

            setCanFAM(FAM >= fullFAM); // is meter full?
        }

        print(FAM + "/100");
    }

    public void setIsGrounded(bool value)
    {
        isGrounded = value;
        animator.SetBool("isGrounded", value);
    }

    public void setOnLedge(bool value)
    {
        onLedge = value;
        animator.SetBool("onLedge", value);
    }

    public void setCanFastFall(bool value)
    {
        canFastFall = value;
        animator.SetBool("canFastFall", value);
    }

    public void setIsAttacking(bool value)
    {
        isAttacking = value;
        animator.SetBool("isAttacking", value);
    }

    public void setIsGettingUp(bool value)
    {
        isGettingUp = value;
        animator.SetBool("isGettingUp", value);
    }

    public void setInHitStun(bool value)
    {
        inHitStun = value;
        animator.SetBool("inHitStun", value);
    }

    public void setCanFAM(bool value)
    {
        canFAM = value;
        animator.SetBool("canFAM", value);
    }

    public void setMovement(float value)
    {
        animator.SetFloat("movement", value);
    }

    public void setIsDoubleJumping(bool value)
    {
        isDoubleJumping = value;
        animator.SetBool("isDoubleJumping", value);
    }

    public void setAttackDirection(string direction)
    {
        resetAttackDirections();
        animator.SetBool(direction, true);
    }

    void resetAttackDirections()
    {
        animator.SetBool("forward", false);
        animator.SetBool("up", false);
        animator.SetBool("down", false);
    }

    public void setAttackStrength(bool strengthLevel)
    {
        animator.SetBool("isLightAttack", strengthLevel);
    }

    public void animateFinalAttack(bool value)
    {
        animator.SetBool("FinalAttacking", value);
    }

    private void OnDestroy() => GameManager.players.Remove(this);
}
