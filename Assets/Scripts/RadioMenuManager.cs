using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioMenuManager : MonoBehaviour
{
    public Transform center;
    public Transform selectObject;

    public GameObject radialMenuRoot;
    bool isRadialMenuActive;

    public TextMeshProUGUI highlightedSpellName;
    public string[] inventorySpellNames;

    // Start is called before the first frame update
    void Start()
    {
        isRadialMenuActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    isRadialMenuActive = !isRadialMenuActive;
        //    if (isRadialMenuActive)
        //    {
        //        radialMenuRoot.SetActive(true);
        //    }
        //    else
        //    {
        //        radialMenuRoot.SetActive(false);
        //    }
        //}

        if (isRadialMenuActive)
        {
            // calculate the option selected.
            // Here we need to change it to react with direction from gamepad and keyboard

            Vector2 delta = center.position - Input.mousePosition;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            angle += 180;

            int currentSpell = 0;

            for (int i = 0; i < 360; i += 90)
            {
                if (angle >= i && angle < i + 90)
                {
                    selectObject.eulerAngles = new Vector3(0, 0, i);
                    highlightedSpellName.text = inventorySpellNames[currentSpell];

                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.Log($"{inventorySpellNames[currentSpell]} spell casted");
                        isRadialMenuActive = false;
                        radialMenuRoot.SetActive(false);
                    }
                }
                currentSpell++;
            }
        }
    }
}
