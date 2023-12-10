using System.Collections;
using Online;
using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    private CharacterStats stats;
    private CharacterAnimate anim;

    private void Awake()
    {
        ComponentsManager cm = GetComponent<ComponentsManager>();
        stats = cm.characterStats; // Reference to Stats
        animator = cm.animator; // Reference to Animator
        anim = cm.charAnim; // Reference to animations
        rb = cm.rigidbody; // Reference to rigidbody
    }
    
    [PunRPC]
    public void playAnimation(string animationName)
    {
        if(anim == null || animator == null)
        {
            ComponentsManager cm = GetComponent<ComponentsManager>();
            animator = cm.animator;
            anim = cm.charAnim;
        }

        anim.animationState = animationName;
        animator.Play(animationName);
        if (animationName == "FinalAttack") animator.Play("FinalSmash", 1);
    }

    public void SyncPlayerData(bool _respawn)
    {
        RoomManager.instance.SendPlayerProperty(
            new Hashtable(){{ConnectToServer.STOCKS, stats.stocks}, 
                            {ConnectToServer.DAMAGE, stats.damage}});
        MatchData.instance.UpdatePlayersData(stats);
        // if (GameManager.usingEditor) Debug.Log("SYNC PLAYER DATA ");
        if(_respawn && stats.stocks > 0) Respawn();
    }

    public void Respawn() => StartCoroutine(RespawnRoutine());
    private IEnumerator RespawnRoutine()
    {
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        stats.inHitStun = false;
        gameObject.transform.position = new Vector3(0.0f, 5.0f, 0.0f);
        rb.velocity = Vector3.zero;
    }
    private void OnDestroy() => GameManager.players.Remove(stats);
}