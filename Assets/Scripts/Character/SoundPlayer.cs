using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource jumpGround;
    public AudioSource run;
    public AudioSource heavyHit;
    public AudioSource lightHit;

    public void playJumpGround() => jumpGround.Play();
    public void playRunSound() => run.Play();      
    public void playHeavyHit() => heavyHit.Play();      
    public void playLightHit() => lightHit.Play();      
}