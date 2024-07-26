using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private Character[] characters;
    public Character[,] slots = new Character[3,2];
    [field:SerializeField] public AudioClip SlotSound { get; private set; }
    [field:SerializeField] public AudioClip SelectionSound { get; private set; }

    // Start is called before the first frame update
    void Awake()
    { 
        int index = 0;
        
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                slots[x,y] = characters[index];

                index++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}

[Serializable]
public class Character
{
    public GameObject image;
    public GameObject prefab;
}