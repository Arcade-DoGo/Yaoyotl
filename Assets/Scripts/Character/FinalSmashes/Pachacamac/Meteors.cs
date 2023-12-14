using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteors : FinalSmash
{
    private float HEIGHT = 8f;

    public override void triggerFinalSmash()
    {
        Vector3 spawnPoint = new Vector3(0f, HEIGHT, 0f);

        Instantiate(
                    attackingObject, // Meteors
                    spawnPoint, // Position
                    Quaternion.Euler(new Vector3(180f, 0f, 0f)) // Direction
                );
    }
}
