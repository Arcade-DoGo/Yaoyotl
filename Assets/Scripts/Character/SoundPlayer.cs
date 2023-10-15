using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{

    public AudioSource jumpGround;
    public AudioSource run;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playJumpGround()
    {
        jumpGround.Play();
    }

    public void playRunSound()
    {
        run.Play();
    }
      
}
