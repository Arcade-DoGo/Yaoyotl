using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectParticles : MonoBehaviour
{

    public float attackDamage = 15f;
    public float baseKnockback = 10f;
    public float hitStunTime = 0.3f;
    public float knockbackAngle;

    private ParticleSystem part;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.GetComponent<ComponentsManager>()) // Checks if the collided gameObject is another player or punchBag
        {
            GameObject player = other.gameObject;
            CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;
            stats.addDamage(attackDamage);
            applyKnockback(player);
            StartCoroutine(applyHitStun(stats));
        }
    }

    void applyKnockback(GameObject player)
    {
        CharacterStats stats = player.GetComponent<ComponentsManager>().characterStats;

        // Calculate launching direction
        Vector3 direction = player.transform.position - transform.position;
        Vector3 knockbackDirection = Quaternion.AngleAxis(knockbackAngle, Vector3.forward) * direction;
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
