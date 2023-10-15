using UnityEngine;

public class isGrounded : MonoBehaviour
{
    public ComponentsManager componentsManager;
    private CharacterStats stats;

    void Start() => stats = componentsManager.characterStats;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Stage") || other.CompareTag("Platform"))
        {
            stats.setIsGrounded(true);
            stats.jumpsUsed = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Stage") || other.CompareTag("Platform"))
            stats.setIsGrounded(false);
    }
}