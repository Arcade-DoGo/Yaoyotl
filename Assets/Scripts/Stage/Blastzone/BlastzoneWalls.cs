using UnityEngine;

public class BlastzoneWalls : MonoBehaviour
{
    Blastzone blastzone;
    public GameObject deathEffect;
    void Start() => blastzone = transform.parent.GetComponent<Blastzone>();
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterStats stats = other.gameObject.GetComponent<ComponentsManager>().characterStats;
            Instantiate(deathEffect, other.transform.position, Quaternion.identity);
            stats.loseStock();
        }
    }
}
