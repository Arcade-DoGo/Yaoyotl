using UnityEngine;

public class isGrounded : MonoBehaviour
{
    public ComponentsManager componentsManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stage") || other.CompareTag("Platform"))
        {
            componentsManager.characterStats.setIsGrounded(true);
            componentsManager.characterStats.jumpsUsed = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stage") || other.CompareTag("Platform"))
            componentsManager.characterStats.setIsGrounded(false);
    }
}