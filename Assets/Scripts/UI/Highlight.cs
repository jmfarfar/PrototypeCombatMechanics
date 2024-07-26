using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Highlight : MonoBehaviour
{
    [SerializeField] Vector2 arrayPosition = Vector2.zero;
    [SerializeField] private Slot slot;
    [SerializeField] private CharacterSelection characterSelection;
    [SerializeField] private GameObject readyImg;

    private IPlayerInput playerInput;

    private GameObject selectedCharacter;

    private Action state;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<IPlayerInput>();
        SetPosition();

        state = SelectPlayerState();
    }

    // Update is called once per frame
    void Update()
    {
        state.Invoke();
    }

    void SetPosition()
    {
        transform.position = slot.slots[(int)arrayPosition.x, (int)arrayPosition.y].image.transform.position;
    }

    Action SelectPlayerState()
    {
        return () =>
        {
            if (playerInput.CharacterSelectionMove(out Vector2 input))
            {
                AudioManager.Instance.PlaySound(slot.SlotSound);
                arrayPosition.y = (((arrayPosition.y + input.y) % 2) + 2) % 2;
                arrayPosition.x = (((arrayPosition.x + input.x) % 3) + 3) % 3;
                SetPosition();
            }

            if (playerInput.UIAccept())
            {
                AudioManager.Instance.PlaySound(slot.SelectionSound);
                state = SelectedState();
            }
        };
    }

    Action SelectedState()
    {
        selectedCharacter = slot.slots[(int)arrayPosition.x, (int)arrayPosition.y].prefab;
        if (!selectedCharacter)
        {
            return SelectPlayerState();
        }
        
        readyImg.SetActive(true);

        if (playerInput.GetType() == typeof(Player1Input))
        {
            characterSelection.Player1 = selectedCharacter;
        }
        else
        {
            characterSelection.Player2 = selectedCharacter;
        }


        return () =>
        {
            if (playerInput.UICancel())
            {
                readyImg.SetActive(false);
                characterSelection.Player1 = null;
                state = SelectPlayerState();
            }
        };
    }
}