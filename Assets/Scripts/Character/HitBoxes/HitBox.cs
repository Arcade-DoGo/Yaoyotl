using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float knockback = 10f;

    void OnTriggerEnter(Collider other)
    {
        GameObject player = other.gameObject;
        string tag = player.tag;

        if (tag == "Player")
        {

            Vector3 knockbackDirection = player.transform.position - transform.position;
            knockbackDirection.Normalize();

            Rigidbody rb = player.GetComponent<Rigidbody>();
            rb.AddForce(knockbackDirection * knockback, ForceMode.Impulse);
        }
    }
}
