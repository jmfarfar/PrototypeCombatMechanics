using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class HoverEventButton : MonoBehaviour
{
    public AudioClip hoverSound;

    public void PlayHoverSound()
    {
        AudioManager.Instance.PlaySound(hoverSound);
    }
}
