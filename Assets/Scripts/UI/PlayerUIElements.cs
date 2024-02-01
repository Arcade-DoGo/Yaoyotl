using System.Numerics;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class PlayerUIElements : MonoBehaviour
{
    public TextMeshProUGUI playerNumber;
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
