using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Based on https://www.youtube.com/watch?v=S3UgE-d2yCI
public class PlayMenuSFX : MonoBehaviour
{
    public AudioSource sfx; 
    public void PlaySound(){
        sfx.Play();
    }
}
