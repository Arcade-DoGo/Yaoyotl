using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [NonSerialized] public bool onLedge, isGrounded, isAttacking, isGettingUp, inHitStun;

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

    private MatchData matchData;

    void Start()
    {
        GameObject hud = GameObject.Find("HUD");
        matchData = hud.GetComponent<MatchData>();
        matchData.updatePlayersData(this);
    }

    public void addDamage(float damage)
    {
        this.damage += damage;
        matchData.updatePlayersData(this);
    }

    public void loseStock()
    {
        stocks--;
        damage = 0;
        matchData.updatePlayersData(this);
    }
}
