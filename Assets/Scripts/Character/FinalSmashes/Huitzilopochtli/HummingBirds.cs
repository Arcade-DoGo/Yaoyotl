using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HummingBirds : FinalSmash
{
    private int WAVES = 3;
    private int FLOCKS = 2;
    private float INTERVALS = 2.5f;
    private float[] HEIGHTS = {1.5f, 4.5f, 7.5f};
    private float WIDTH = 20f;
    private float MOVEMENT_SPEED = 8f;

    public override void triggerFinalSmash()
    {
        StartCoroutine(performFinalSmash()); 
    }

    private IEnumerator performFinalSmash()
    {
        for (int wave = 0; wave < WAVES; wave++)
        {
            List<float> heights = new List<float>(HEIGHTS);
            heights.RemoveAt(Random.Range( 0, HEIGHTS.Length ));

            for (int flock = 0; flock < FLOCKS; flock++)
            {
                Vector3 spawnPoint = new Vector3(-WIDTH, heights[flock], 4);

                GameObject flockObject = Instantiate(
                    attackingObject, // Flock
                    spawnPoint, // Position
                    Quaternion.identity // Direction
                );

                flockObject.GetComponent<Rigidbody>().velocity = new Vector3(MOVEMENT_SPEED, 0.0f, 0.0f);
            }

            yield return new WaitForSeconds(INTERVALS);
        }
    }
}
