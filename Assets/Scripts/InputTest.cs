using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    void Update()
    {
        Vector2 inputVector = new Vector2(Input.GetAxisRaw("JRHorizontal"), Input.GetAxisRaw("JRVertical"));

        float angle = Mathf.Atan2(inputVector.y,inputVector.x) * Mathf.Rad2Deg;

        angle = Mathf.Floor(((angle + 360) % 360 / 90) % 3);
    }
}
