using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Flamethrower : MonoBehaviour, IAnimationEffects
{
    public VisualEffect flamethrower;
    public AudioClip flamethrowerAudioClip;    
   

    public void StartEffect()
    {        
        flamethrower.Play();
        AudioManager.Instance.PlaySound(flamethrowerAudioClip);
    }

    public void EndEffect()
    {     
        flamethrower.Stop();
    }
}
