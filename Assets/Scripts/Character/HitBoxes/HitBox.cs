using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float baseKnockback = 10f;
    public float attackDamage = 3.5f;
    public float hitStunTime = 0.3f;

    void OnTriggerEnter(Collider other)
    {
        GameObject player = other.gameObject;
        CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;
        string tag = player.tag;

        if (tag == "Player" && player != transform.parent) // Collides with other player (Not the player that owns it)
        {
            stats.addDamage(attackDamage);

            applyKnockback(player);

            StartCoroutine(applyHitStun(stats));
        }
    }

    void applyKnockback(GameObject player)
    {
        CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;

        // Calculate launching direction
        Vector3 knockbackDirection = player.transform.position - transform.position;
        knockbackDirection.Normalize();

        // Calculate launching force
        float additionalKnockback = baseKnockback * (stats.damage / 100f); // Base knockback * x.xx (xxx%)
        float knockbackForce = baseKnockback + additionalKnockback;

        Rigidbody rb = player.GetComponent<ComponentsManager>().rigidbody;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
    }

    IEnumerator applyHitStun(CharacterStats stats)
    {
        stats.inHitStun = true;
        yield return new WaitForSeconds(hitStunTime);
        stats.inHitStun = false;
    }
}
