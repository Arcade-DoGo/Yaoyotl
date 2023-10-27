using UnityEngine;

public class isGrounded : MonoBehaviour
{
    public ComponentsManager componentsManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stage") || other.CompareTag("Platform"))
        {
            componentsManager.characterStats.isGrounded = true;
            componentsManager.characterStats.jumpsUsed = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stage") || other.CompareTag("Platform"))
            componentsManager.characterStats.isGrounded = false;
    }
}