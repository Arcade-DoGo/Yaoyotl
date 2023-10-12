using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [NonSerialized] public bool onLedge, isGrounded, isAttacking, isGettingUp, inHitStun, canFAM;

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

    private MatchData matchData;

    private void Awake()
    {
        playerName = GetComponent<ComponentsManager>().photonView.Owner.NickName;
        playerNumber = GetComponent<ComponentsManager>().photonView.Owner.ActorNumber;
        GameManager.players.Add(this);
        if(GameManager.usingEditor) Debug.Log("Added " + playerName + " in " + GameManager.players.Count);
    }
    void Start()
    {
        GameObject hud = GameObject.Find("HUD");
        matchData = hud.GetComponent<MatchData>();
        matchData.updatePlayersData(this);
        
        StartCoroutine(chargeFAM());
    }

    public void addDamage(float damage)
    {
        this.damage += damage;
        increaseFAM(damage / 5f);
        print("FAM Damage Increase--------------------");
        matchData.updatePlayersData(this);
    }

    public void loseStock()
    {
        stocks--;
        damage = 0;
        FAM /= 2f;
        matchData.updatePlayersData(this);
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

            // print(FAM + "/" + fullFAM);
        }
    }
}
