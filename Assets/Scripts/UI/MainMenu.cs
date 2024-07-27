using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("UI_MainMenu");
    }
    
    public void LoadCharacterSelectScreen()
    {
        SceneManager.LoadScene("UI_CharacterSelect_menu");
    }

    public void LoadCreditsScreen()
    {
        
    }

    public void LoadControlsScreen()
    {
        SceneManager.LoadScene("UI_Cotrol_Menu");
    }

    public void LoadArena()
    {
        SceneManager.LoadScene("AttackAnimations");
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
