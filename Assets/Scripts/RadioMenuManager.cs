using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioMenuManager : MonoBehaviour
{
    public Transform radioMenuPlayer1;
    public Transform radioMenuPlayer2;
    public float deactivationTime;
    private PlayerStateMachine[] players;
    private bool isMenuExecuting;
    private Color[] menuColors = {new(0f, 0.8f, 1f), new(1f, 0.6f, 0.3f), new(0.9f, 0.2f, 0.3f), Color.white};

    private void Start() 
    {
        players = FindObjectsOfType<PlayerStateMachine>();

        foreach (PlayerStateMachine player in players)
        {
            player.onRadialMenuActivated += OnRadialMenuActivated;
            player.onRadialMenuExecuted += OnRadialMenuExecuted;
        }
    }

    private void OnRadialMenuExecuted(LayerMask layerMask, string spellName, int cuadrant)
    {
        if (layerMask == LayerMask.NameToLayer("Player1Hurtbox"))
        {
            DeactivateRadioMenu(radioMenuPlayer1, cuadrant, spellName);
        }
        else if (layerMask == LayerMask.NameToLayer("Player2Hurtbox"))
        {
            DeactivateRadioMenu(radioMenuPlayer2, cuadrant, spellName);
        }
    }

    private void OnRadialMenuActivated(LayerMask layerMask, int index, bool isActive)
    {
        if (isMenuExecuting && !isActive){return;}

        if (layerMask == LayerMask.NameToLayer("Player1Hurtbox"))
        {
            ToogleRadioMenu(radioMenuPlayer1.gameObject, isActive, index);
        }
        else if (layerMask == LayerMask.NameToLayer("Player2Hurtbox"))
        {
            ToogleRadioMenu(radioMenuPlayer2.gameObject, isActive, index);
        }
    }

    private void ToogleRadioMenu(GameObject menu, bool isActive, int index)
    {
        menu.transform.GetChild(1).GetComponent<Image>().color = menuColors[index];
        menu.SetActive(isActive);
        menu.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void DeactivateRadioMenu(Transform menu, int cuadrant, string spellName)
    {
        GameObject selectedSprite = menu.GetChild(2).gameObject;
        selectedSprite.SetActive(true);
        selectedSprite.transform.rotation = Quaternion.Euler(0f, 0f, cuadrant * 90f);
        //menu.GetChild(7).GetComponent<TextMeshProUGUI>().text = spellName;
        StartCoroutine(DeactivateRadioMenuRoutine(menu.gameObject, deactivationTime));
    }

    IEnumerator DeactivateRadioMenuRoutine(GameObject menu, float time)
    {
        isMenuExecuting = true;
        yield return new WaitForSeconds(time);
        menu.SetActive(false);
        isMenuExecuting = false;
        //menu.transform.GetChild(1).GetComponent<Image>().color = menuColors[3];
        //menu.transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "Attack name";
    }
}
