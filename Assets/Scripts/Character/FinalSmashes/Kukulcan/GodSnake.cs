using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodSnake : FinalSmash
{
    private float DURATION = 30f;

    public override void  triggerFinalSmash()
    {
        StartCoroutine(performFinalSmash());
    }

    private IEnumerator performFinalSmash() 
    {
        GameObject snake = Instantiate(
                    attackingObject, // Snake
                    new Vector3(0f, 2.5f, 1.5f), // Position
                    Quaternion.identity // Direction
                );

        yield return new WaitForSeconds(DURATION);

        Destroy(snake);
    }
}
