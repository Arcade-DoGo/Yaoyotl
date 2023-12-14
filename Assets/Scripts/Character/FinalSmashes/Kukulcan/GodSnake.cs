using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodSnake : FinalSmash
{

    private float DURATION = 11f;

    public override void  triggerFinalSmash()
    {
        StartCoroutine(performFinalSmash());
    }

    private IEnumerator performFinalSmash()
    {
        attackingObject.SetActive(true);

        yield return new WaitForSeconds(DURATION);

        attackingObject.SetActive(false);
    }
}
