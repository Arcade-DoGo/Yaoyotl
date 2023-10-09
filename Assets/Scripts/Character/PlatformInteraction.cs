using UnityEngine;

public class PlatformInteraction : MonoBehaviour
{
    private Rigidbody rb;

    void Start() => rb = GetComponent<ComponentsManager>().rigidbody;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            if (rb.velocity.y <= 0) // Falling on the plaform
                gameObject.layer = LayerMask.NameToLayer("CharacterLayer");
        }
    }
}