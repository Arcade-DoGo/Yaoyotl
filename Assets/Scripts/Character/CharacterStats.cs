using System;
using System.Collections;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using Online;

public class CharacterStats : MonoBehaviour
{
    [Header ("Player Status")]
    [NonSerialized] public bool onLedge, isGrounded, isDoubleJumping, canFastFall, isAttacking, isGettingUp, inHitStun, canFAM;
    [NonSerialized] public bool isFacingRight = true;

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
    [NonSerialized] public int jumpsUsed = 0;

    [Header ("Private variables")]
    private float FAM = 0f;
    private float fullFAM = 100f;
    private int secondsTillFAM = 10; // 2.5 minutes

    [Header ("Private components")]
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
        canFAM = false;
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

            canFAM = FAM >= fullFAM; // is meter full?
        }

        // print(FAM + "/100");
    }

    private void OnDestroy() => GameManager.players.Remove(this);
}