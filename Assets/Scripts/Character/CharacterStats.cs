using System;
using System.Collections;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
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
    private int secondsTillFAM = 10; // 2.5 minutes

    private Rigidbody rb;
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
        rb = GetComponent<ComponentsManager>().rigidbody;
        animator = GetComponent<ComponentsManager>().animator;
        MatchData.instance.UpdatePlayersData(this);

        StartCoroutine(chargeFAM());
    }

    public void addDamage(float _damage)
    {
        if(PhotonNetwork.IsConnected)
        {
            if(photonView.IsMine)
            {
                damage += _damage;
                increaseFAM(damage / 5f);
                SyncPlayerData();
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
        if(PhotonNetwork.IsConnected)
        {
            if(photonView.IsMine)
            {
                stocks--;
                damage = 0;
                FAM /= 2f;
                SyncPlayerData();
            }
        }
        else
        {
            stocks--;
            damage = 0;
            FAM /= 2f;
            MatchData.instance.UpdatePlayersData(this);
            Respawn();
        }
    }

    
    private void SyncPlayerData()
    {
        //Stocks
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ConnectToServer.STOCKS)) PhotonNetwork.LocalPlayer.CustomProperties.Add(ConnectToServer.STOCKS, stocks);
        PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.STOCKS] = stocks;
        //Damage
        if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(ConnectToServer.DAMAGE)) PhotonNetwork.LocalPlayer.CustomProperties.Add(ConnectToServer.DAMAGE, damage);
        PhotonNetwork.LocalPlayer.CustomProperties[ConnectToServer.DAMAGE] = damage;
        PhotonNetwork.SetPlayerCustomProperties(new Hashtable() { { ConnectToServer.STOCKS, stocks }, { ConnectToServer.DAMAGE, damage } });
        MatchData.instance.UpdatePlayersData(this);
        print("SYNC PLAYER DATA ");
        Respawn();
    }
    public void Respawn() => StartCoroutine(RespawnRoutine());
    private IEnumerator RespawnRoutine()
    {
        if (stocks > 0)
        {
            rb.velocity = Vector3.zero;
            yield return new WaitForSeconds(0.5f);
            inHitStun = false;
            gameObject.transform.position = new Vector3(0.0f, 5.0f, 0.0f);
            rb.velocity = Vector3.zero;
        }
        else // Game Over: Show winner
            GameManager.instance.GameOver(GameManager.players.Find(player => player != this));
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

        // print(FAM + "/100");
    }

    public void setIsGrounded(bool value)
    {
        isGrounded = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("isGrounded", value);
    }

    public void setOnLedge(bool value)
    {
        onLedge = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("onLedge", value);
    }

    public void setCanFastFall(bool value)
    {
        canFastFall = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("canFastFall", value);
    }

    public void setIsAttacking(bool value)
    {
        isAttacking = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("isAttacking", value);
    }

    public void setIsGettingUp(bool value)
    {
        isGettingUp = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("isGettingUp", value);
    }

    public void setInHitStun(bool value)
    {
        inHitStun = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("inHitStun", value);
    }

    public void setCanFAM(bool value)
    {
        canFAM = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("canFAM", value);
    }

    public void setMovement(float value)
    {
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetFloat("movement", value);
    }

    public void setIsDoubleJumping(bool value)
    {
        isDoubleJumping = value;
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("isDoubleJumping", value);
    }

    public void setAttackDirection(string direction)
    {
        resetAttackDirections();
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool(direction, true);
    }

    void resetAttackDirections()
    {
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("forward", false);
        animator.SetBool("up", false);
        animator.SetBool("down", false);
    }

    public void setAttackStrength(bool strengthLevel)
    {
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("isLightAttack", strengthLevel);
    }

    public void animateFinalAttack(bool value)
    {
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("FinalAttacking", value);
    }

    public void setIsFalling(bool value)
    {
        if(animator == null) animator = GetComponent<ComponentsManager>().animator;
        animator.SetBool("isFalling", value);
    }

    private void OnDestroy() => GameManager.players.Remove(this);
}