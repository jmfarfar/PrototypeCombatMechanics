using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private void Awake()
    {
        if (AudioManager.Instance.IsPlaying())
        {
            AudioManager.Instance.StopClip();
            AudioManager.Instance.PlayAmbiental();

        }
    }
}
