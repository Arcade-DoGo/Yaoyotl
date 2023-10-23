using UnityEngine;

public class BlastzoneWalls : MonoBehaviour
{
    public AudioSource audioSource;
    public GameObject deathEffect;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.Play();
            CharacterStats stats = other.gameObject.GetComponent<ComponentsManager>().characterStats;
            Instantiate(deathEffect, other.transform.position, Quaternion.identity);
            stats.loseStock();
        }
    }
}
